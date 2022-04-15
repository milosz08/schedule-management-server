using AutoMapper;

using System.Net;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using asp_net_po_schedule_management_server.Jwt;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Dto.Misc;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Dto.AuthDtos;


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
                throw new BasicServerException("Podany użytkownik nie istenieje w bazie.", HttpStatusCode.NotFound);
            }
            
            PasswordVerificationResult verificatrionRes = _passwordHasher
                .VerifyHashedPassword(findPerson, findPerson.Password, user.Password);
            
            if (verificatrionRes == PasswordVerificationResult.Failed) {
                throw new BasicServerException("Podano zły login lub hasło. Spróbuj ponownie", HttpStatusCode.Unauthorized);
            }
            
            return new LoginResponseDto() 
            {
                BearerToken = _manager.BearerHandlingService(findPerson),
                Role = findPerson.Role.Name,
            };
        }

        #endregion
        
        #region Register

        // metoda odpowiadająca za stworzenie nowego użytkownika i dodanie go do bazy danych
        public async Task<RegisterNewUserResponseDto> UserRegister(RegisterNewUserRequestDto user)
        {
            string generatedShortcut = user.Name.Substring(0, 3) + user.Surname.Substring(0, 3);
            string generatedLogin = generatedShortcut.ToLower() + ApplicationUtils.DictionaryHashGenerator(5);
            string generatedFirstPassword = ApplicationUtils.DictionaryHashGenerator(8);
            string generatedEmail = $"{user.Name.ToLower()}.{user.Surname.ToLower()}@schedule.pl";

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
        public async Task<PseudoNoContentResponseDto> UserChangePassword(ChangePasswordRequestDto dto, string userId)
        {
            Person findPerson = await _context.Persons.FirstOrDefaultAsync(p => p.DictionaryHash == userId);
            if (findPerson == null) {
                throw new BasicServerException($"Nie znaleziono użytkownika w bazie danych", HttpStatusCode.NotFound);
            }
            
            PasswordVerificationResult verificationPassword = _passwordHasher
                .VerifyHashedPassword(findPerson, findPerson.Password, dto.OldPassword);
            if (verificationPassword == PasswordVerificationResult.Failed) {
                throw new BasicServerException("Podano złe hasło pierwotne", HttpStatusCode.Unauthorized);
            }
            
            findPerson.Password = _passwordHasher.HashPassword(findPerson, dto.NewPassword);
            findPerson.FirstAccess = false;
            _context.Persons.Update(findPerson);
            await _context.SaveChangesAsync();
            
            return new PseudoNoContentResponseDto()
            {
                Message = $"Hasło dla użytkownika {findPerson.Name} {findPerson.Surname} zostało pomyślnie zmienione"
            };
        }

        #endregion
    }
}