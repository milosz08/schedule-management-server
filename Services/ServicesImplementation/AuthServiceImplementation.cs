using AutoMapper;

using System;
using System.Net;
using System.Linq;
using System.Text;
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
using asp_net_po_schedule_management_server.Dto.CrossQuery;
using asp_net_po_schedule_management_server.Ssh.SshEmailService;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public class AuthServiceImplementation : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtAuthenticationManager _manager;
        private readonly IPasswordHasher<Person> _passwordHasher;
        private readonly IMapper _mapper;
        
        public AuthServiceImplementation(
            ApplicationDbContext context,
            IJwtAuthenticationManager manager,
            IPasswordHasher<Person> passwordHasher,
            IMapper mapper)
        {
            _context = context;
            _manager = manager;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }
        
        #region Login

        // metoda odpowiadająca za zalogowanie użytkownika (jeśli login niepoprawny, rzuci wyjątek)
        public async Task<LoginResponseDto> UserLogin(LoginRequestDto user)
        {
            Person findPerson = await _context.Persons
                .Include(p => p.Role)
                .FirstOrDefaultAsync(p => p.Login == user.Login);

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
            return response;
        }
        
        #endregion

        
        #region Refresh Token
        
        // metoda odpowiadająca za odświeżanie JWT na podstawie tokenu odświeżającego
        public async Task<RefreshTokenDto> UserRefreshToken(RefreshTokenDto dto)
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
            
            // stworzenie nowego tokenu odświeżającego oraz JWT i wysyłka do klienta
            string bearerRefreshToken = _manager.RefreshTokenGenerator();
            findRefreshToken.TokenValue = bearerRefreshToken;
            await _context.SaveChangesAsync();
            
            return new RefreshTokenDto()
            {
                BearerToken = _manager.BearerHandlingRefreshTokenService(principal.Claims.ToArray()),
                RefreshBearerToken = bearerRefreshToken,
            };
        }

        #endregion

        
        #region Register

        // metoda odpowiadająca za stworzenie nowego użytkownika i dodanie go do bazy danych
        public async Task<RegisterNewUserResponseDto> UserRegister(RegisterNewUserRequestDto user)
        {
            string generatedShortcut = user.Name.Substring(0, 3) + user.Surname.Substring(0, 3);
            string generatedLogin = generatedShortcut.ToLower() + ApplicationUtils.RandomNumberGenerator();
            string generatedFirstPassword = ApplicationUtils.GenerateUserFirstPassword();
            string generatedEmail = $"{user.Name.ToLower()}.{user.Surname.ToLower()}" +
                                    $"{ApplicationUtils.RandomNumberGenerator()}@" +
                                    $"{GlobalConfigurer.UserEmailDomain}";
            
            _emailService.AddNewEmailAccount(generatedEmail, generatedFirstPassword);

            Role findRoleId = await _context.Roles
                .FirstOrDefaultAsync(role => role.Name == nameof(AvailableRoles.TEACHER));

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
            response.Role = nameof(AvailableRoles.TEACHER);
            return response;
        }

        #endregion
        
        
        #region ChangePassword

        // metoda odpowiadająca za zmianę początkowego hasła przez użytkownika
        public async Task<PseudoNoContentResponseDto> UserChangePassword(
            ChangePasswordRequestDto dto, string userId, Claim userLogin)
        {
            Person findPerson = await _context.Persons.FirstOrDefaultAsync(p => p.DictionaryHash == userId);
            if (findPerson == null) {
                throw new BasicServerException($"Nie znaleziono użytkownika w bazie danych.", HttpStatusCode.NotFound);
            }

            // jeśli login zapisany w tokenie JWT nie jest zgody ze znalezionym użytkownikiem
            if (findPerson.Login != userLogin.Value) {
                throw new BasicServerException("Brak poświadczeń do edycji zasobu.", HttpStatusCode.Forbidden);
            }
            
            PasswordVerificationResult verificationPassword = _passwordHasher
                .VerifyHashedPassword(findPerson, findPerson.Password, dto.OldPassword);
            if (verificationPassword == PasswordVerificationResult.Failed) {
                throw new BasicServerException("Podano złe hasło pierwotne.", HttpStatusCode.Unauthorized);
            }
            
            findPerson.Password = _passwordHasher.HashPassword(findPerson, dto.NewPassword);
            findPerson.FirstAccess = false;
            _context.Persons.Update(findPerson);
            await _context.SaveChangesAsync();
            
            return new PseudoNoContentResponseDto()
            {
                Message = $"Hasło dla użytkownika {findPerson.Name} {findPerson.Surname} zostało pomyślnie zmienione."
            };
        }

        #endregion
    }
}