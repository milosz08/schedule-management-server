using System;
using AutoMapper;

using System.Net;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using asp_net_po_schedule_management_server.Jwt;
using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Services.Helpers;
using asp_net_po_schedule_management_server.Ssh.SshEmailService;
using asp_net_po_schedule_management_server.Ssh.SmtpEmailService;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class AuthServiceImplementation : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly ServiceHelper _helper;
        private readonly ApplicationDbContext _context;
        private readonly ISshEmailService _emailService;
        private readonly IJwtAuthenticationManager _manager;
        private readonly ISmtpEmailService _smtpEmailService;
        private readonly IPasswordHasher<Person> _passwordHasher;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public AuthServiceImplementation(IMapper mapper, ServiceHelper helper, ApplicationDbContext context,
            ISshEmailService emailService, IJwtAuthenticationManager manager, ISmtpEmailService smtpEmailService,
            IPasswordHasher<Person> passwordHasher)
        {
            _mapper = mapper;
            _helper = helper;
            _context = context;
            _manager = manager;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _smtpEmailService = smtpEmailService;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Login

        /// <summary>
        /// Metoda odpowiadająca za zalogowanie użytkownika (jeśli login niepoprawny, rzuci wyjątek).
        /// </summary>
        /// <param name="user">obiekt transferowy z danymi logowania</param>
        /// <returns>zwraca odpowiedni status zalogowania lub status błędu (brak autoryzacji)</returns>
        /// <exception cref="BasicServerException">w przypadku braku znalezienia zasobu / dozwolonej operacji</exception>
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
        
        /// <summary>
        /// Metoda odpowiadająca za odświeżanie JWT na podstawie tokenu odświeżającego.
        /// </summary>
        /// <param name="dto">reprezentacja danych od klienta w postaci zapytania z JWT i tokenem odświeżającym</param>
        /// <returns>nowy token JWT i token odświeżający</returns>
        /// <exception cref="BasicServerException">w przypadku braku znalezienia zasobu / dozwolonej operacji</exception>
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

        /// <summary>
        /// Metoda odpowiadająca za stworzenie nowego użytkownika i dodanie go do bazy danych.
        /// </summary>
        /// <param name="user">reprezentacja obiektowa użytkownika</param>
        /// <param name="customPassword">domyślne hasło (używane tylko dla domyślnego użytkownika)</param>
        /// <returns>informacje o zarejestrowanym użytkowniku</returns>
        public async Task<RegisterNewUserResponseDto> UserRegister(RegisterNewUserRequestDto user, string customPassword)
        {
            RegisterUserGeneratedValues defValues = _helper.GenerateUserDetails(user);
            _emailService.AddNewEmailAccount(defValues.Email, defValues.EmailPassword);
            
            Role findRoleId = await _context.Roles.FirstOrDefaultAsync(role => role.Name == user.Role);
            if (findRoleId == null) {
                throw new BasicServerException("Podana rola nie istnieje w systemie.", HttpStatusCode.NotFound);
            }

            if (customPassword != String.Empty) {
                defValues.Password = customPassword;
            }

            // wyszukaj pasującego wydziału, jeśli nie znajdzie dodaj bez wydziału
            Department findDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.Name.Equals(user.DepartmentName, StringComparison.OrdinalIgnoreCase));

            // wyszukaj pasującą katedrę (na podstawie wydziału), jeśli nie znajdzie dodaj bez katedry
            Cathedral findCathedral = await _context.Cathedrals
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.Name.Equals(user.CathedralName, StringComparison.OrdinalIgnoreCase) &&
                                          c.Department.Name.Equals(findDepartment.Name, StringComparison.OrdinalIgnoreCase));

            Person newPerson = new Person()
            {
                Name =  ApplicationUtils.CapitalisedLetter(user.Name),
                Surname = ApplicationUtils.CapitalisedLetter(user.Surname),
                Nationality = ApplicationUtils.CapitalisedLetter(user.Nationality),
                City = ApplicationUtils.CapitalisedLetter(user.City),
                Shortcut = defValues.Shortcut,
                Email = defValues.Email,
                Login = defValues.Login,
                Password = defValues.Password,
                EmailPassword = defValues.EmailPassword,
                RoleId = findRoleId.Id,
                IfRemovable = user.IfRemovable,
                DepartmentId = findDepartment.Id,
                CathedralId = findCathedral == null ? null : findCathedral.Id,
            };
            
            // mapowanie relacyjne przedmiotów i kierunków
            if (user.Role != AvailableRoles.ADMINISTRATOR) {
                if (user.Role == AvailableRoles.STUDENT) { // kierunki dla studentów
                    IEnumerable<StudySpecialization> findAllSpecializations = _context.StudySpecializations
                        .Include(s => s.StudyType)
                        .Where(s => user.StudySpecsOrSubjects.Any(id => id == s.Id))
                        .AsEnumerable();
                    newPerson.StudySpecializations = findAllSpecializations.ToList();
                } else { // przedmioty dla nauczycieli i edytorów
                    IEnumerable<StudySubject> findAllStudySubjects = _context.StudySubjects
                        .Where(b => user.StudySpecsOrSubjects.Any(id => id == b.Id))
                        .AsEnumerable();
                    newPerson.Subjects = findAllStudySubjects.ToList();
                }
            }
            
            newPerson.Password = _passwordHasher.HashPassword(newPerson, defValues.Password);
            await _context.Persons.AddAsync(newPerson);
            await _context.SaveChangesAsync();

            // wysłanie do użytkownika emailu zawierającego wszystkie niezbędne dane do logowania (pomijanie
            // użytkownika domyślnego)
            if (customPassword == String.Empty) {
                await _smtpEmailService.SendCreatedUserAuthUser(new UserEmailOptions()
                {
                    ToEmails = new List<string>() {newPerson.Email},
                    Placeholders = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("{{userName}}", $"{newPerson.Name} {newPerson.Surname}"),
                        new KeyValuePair<string, string>("{{login}}", newPerson.Login),
                        new KeyValuePair<string, string>("{{password}}", defValues.Password),
                        new KeyValuePair<string, string>("{{role}}", newPerson.Role.Name),
                        new KeyValuePair<string, string>("{{serverTime}}", ApplicationUtils.GetCurrentUTCdateString()),
                        new KeyValuePair<string, string>("{{dictionaryHash}}", newPerson.DictionaryHash),
                    },
                });    
            }
            return _mapper.Map<RegisterNewUserResponseDto>(newPerson);
        }

        #endregion
    }
}