using AutoMapper;

using System;
using System.Net;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using asp_net_po_schedule_management_server.Jwt;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Exceptions;

using asp_net_po_schedule_management_server.Dto.Requests;
using asp_net_po_schedule_management_server.Dto.Responses;

using asp_net_po_schedule_management_server.Ssh.SshEmailService;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public class AuthServiceImplementation : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtAuthenticationManager _manager;
        private readonly IPasswordHasher<Person> _passwordHasher;
        private readonly IMapper _mapper;
        private readonly ISshEmailService _emailService;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public AuthServiceImplementation(
            ApplicationDbContext context,
            IJwtAuthenticationManager manager,
            IPasswordHasher<Person> passwordHasher,
            IMapper mapper,
            ISshEmailService emailService)
        {
            _context = context;
            _manager = manager;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Login

        // metoda odpowiadająca za zalogowanie użytkownika (jeśli login niepoprawny, rzuci wyjątek)
        public async Task<LoginResponseDto> UserLogin(LoginRequestDto user)
        {
            Person findPerson = await _context.Persons
                .Include(p => p.Role)
                .FirstOrDefaultAsync(p => p.Login == user.Login || p.Email == user.Login);

            if (findPerson == null) {
                throw new BasicServerException("Podany użytkownik nie istenieje w systemie.", HttpStatusCode.NotFound);
            }
            
            PasswordVerificationResult verificatrionRes = _passwordHasher
                .VerifyHashedPassword(findPerson, findPerson.Password, user.Password);
            
            if (verificatrionRes == PasswordVerificationResult.Failed) {
                throw new BasicServerException("Podano zły login lub hasło. Spróbuj ponownie.", HttpStatusCode.Unauthorized);
            }
            
            string bearerRefreshToken;

            RefreshToken findRefreshToken = await _context.Tokens.FirstOrDefaultAsync(t => t.PersonId == findPerson.Id);
            if (findRefreshToken == null) {
                bearerRefreshToken = _manager.RefreshTokenGenerator();
                RefreshToken refreshToken = new RefreshToken()
                {
                    TokenValue = bearerRefreshToken,
                    PersonId = findPerson.Id,
                };
                await _context.Tokens.AddAsync(refreshToken);
                await _context.SaveChangesAsync();
            } else {
                bearerRefreshToken = findRefreshToken.TokenValue;
            }

            LoginResponseDto response = _mapper.Map<LoginResponseDto>(findPerson);
            response.BearerToken = _manager.BearerHandlingService(findPerson);
            response.TokenExpirationDate = DateTime.UtcNow.Add(GlobalConfigurer.JwtExpiredTimestamp);
            response.RefreshBearerToken = bearerRefreshToken;
            response.tokenRefreshInSeconds = GlobalConfigurer.JwtExpiredTimestamp.TotalSeconds;
            return response;
        }
        
        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region Refresh Token
        
        // metoda odpowiadająca za odświeżanie JWT na podstawie tokenu odświeżającego
        public async Task<RefreshTokenResponseDto> UserRefreshToken(RefreshTokenRequestDto dto)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            ClaimsPrincipal principal;

            try {
                // walidacja tokena JWT poprze uzyskanie obiektu principals
                principal = tokenHandler.ValidateToken(
                    dto.BearerToken,
                    JwtAuthenticationManagerImplementation.GetBasicTokenValidationParameters(false),
                    out validatedToken
                );
            
                JwtSecurityToken jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null || !jwtToken.Header.Alg
                        .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
                    throw new BasicServerException("Niepoprawny token.", HttpStatusCode.ExpectationFailed);
                }
            }
            catch (Exception ex) {
                throw new BasicServerException("Nieoczekiwany błąd podczas odczytywania tokenu.",
                    HttpStatusCode.ExpectationFailed);
            }
            
            // wyszukanie, czy podany token odświeżający istnieje, jeśli nie rzuć wyjącek 403 forbidden
            RefreshToken findRefreshToken = await _context.Tokens
                .Include(p => p.Person)
                .FirstOrDefaultAsync(t => t.TokenValue == dto.RefreshBearerToken && t.PersonId == t.Person.Id);
            if (findRefreshToken == null) {
                throw new BasicServerException("Nie znaleziono tokenu odświeżającego.", HttpStatusCode.Forbidden);
            }

            return new RefreshTokenResponseDto()
            {
                BearerToken = _manager.BearerHandlingRefreshTokenService(principal.Claims.ToArray()),
                RefreshBearerToken = findRefreshToken.TokenValue,
                TokenExpirationDate = DateTime.UtcNow.Add(GlobalConfigurer.JwtExpiredTimestamp),
                tokenRefreshInSeconds = GlobalConfigurer.JwtExpiredTimestamp.TotalSeconds,
            };
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region Register

        // metoda odpowiadająca za stworzenie nowego użytkownika i dodanie go do bazy danych
        public async Task<RegisterNewUserResponseDto> UserRegister(RegisterNewUserRequestDto user,
            string customPassword = "", AvailableRoles defRole = AvailableRoles.TEACHER)
        {
            string nameWithoutDiacritics = ApplicationUtils.RemoveAccents(user.Name);
            string surnameWithoutDiacritics = ApplicationUtils.RemoveAccents(user.Surname);
            
            string generatedShortcut = nameWithoutDiacritics.Substring(0, 3) + surnameWithoutDiacritics.Substring(0, 3);
            string randomNumbers = ApplicationUtils.RandomNumberGenerator();
            string generatedLogin = generatedShortcut.ToLower() + randomNumbers;
            string generatedFirstPassword = ApplicationUtils.GenerateUserFirstPassword();
            string generatedFirstEmailPassword = ApplicationUtils.GenerateUserFirstPassword();
            string generatedEmail = $"{nameWithoutDiacritics.ToLower()}.{surnameWithoutDiacritics.ToLower()}" +
                                    $"{randomNumbers}@{GlobalConfigurer.UserEmailDomain}";
            
            _emailService.AddNewEmailAccount(generatedEmail, generatedFirstEmailPassword);

            Role findRoleId = await _context.Roles
                .FirstOrDefaultAsync(role => role.Name == defRole.ToString());

            if (customPassword != String.Empty) {
                generatedFirstPassword = customPassword;
            }
            
            Person newPerson = _mapper.Map<Person>(user);
            newPerson.Shortcut = generatedShortcut;
            newPerson.Email = generatedEmail;
            newPerson.Login = generatedLogin;
            newPerson.Password = generatedFirstPassword;
            newPerson.RoleId = findRoleId.Id;
            
            newPerson.Password = _passwordHasher.HashPassword(newPerson, generatedFirstPassword);
            await _context.Persons.AddAsync(newPerson);
            await _context.SaveChangesAsync();
            
            RegisterNewUserResponseDto response = _mapper.Map<RegisterNewUserResponseDto>(newPerson);
            response.Password = generatedFirstPassword;
            response.EmailPassword = generatedFirstEmailPassword;
            response.Role = nameof(defRole);
            return response;
        }

        #endregion
    }
}