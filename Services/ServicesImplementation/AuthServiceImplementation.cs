﻿using AutoMapper;

using System.Net;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using asp_net_po_schedule_management_server.Jwt;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.DbConfig;
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

        
        // metoda odpowiadająca za zalogowanie użytkownika (jeśli login niepoprawny, rzuci wyjątek)
        public LoginResponseDto UserLogin(LoginRequestDto user)
        {
            Task<Person> findPerson = _context.Persons
                .Include(p => p.Role)
                .FirstOrDefaultAsync(p => p.Login == user.Login);

            if (findPerson.Result == null) {
                throw new BasicServerException("Podany użytkownik nie istenieje w bazie.", HttpStatusCode.NotFound);
            }
            
            var verificatrionRes = _passwordHasher
                .VerifyHashedPassword(findPerson.Result, findPerson.Result.Password, user.Password);
            
            if (verificatrionRes == PasswordVerificationResult.Failed) {
                throw new BasicServerException("Podano zły login lub hasło. Spróbuj ponownie",
                    HttpStatusCode.Unauthorized);
            }
            
            return new LoginResponseDto() {
                BearerToken = _manager.BearerHandlingService(findPerson.Result),
                Role = findPerson.Result.Role.Name
            };
        }

        
        // metoda odpowiadająca za stworzenie nowego użytkownika i dodanie go do bazy danych
        public RegisterNewUserResponseDto UserRegister(RegisterNewUserRequestDto user)
        {
            string generatedShortcut = user.Name.Substring(0, 3) + user.Surname.Substring(0, 3);
            string generatedLogin = generatedShortcut.ToLower() + ApplicationUtils.DictionaryHashGenerator(5);
            string generatedFirstPassword = ApplicationUtils.DictionaryHashGenerator(8);
            string generatedEmail = $"{user.Name.ToLower()}.{user.Surname.ToLower()}@schedule.pl";

            long findRoleId = _context.Roles
                .FirstOrDefaultAsync(role => role.Name == nameof(AvailableRoles.TEACHER)).Result.Id;

            Person newPerson = _mapper.Map<Person>(user);
            newPerson.Shortcut = generatedShortcut;
            newPerson.Email = generatedEmail;
            newPerson.Login = generatedLogin;
            newPerson.Password = generatedFirstPassword;
            newPerson.RoleId = findRoleId;
            
            newPerson.Password = _passwordHasher.HashPassword(newPerson, generatedFirstPassword);
            _context.Persons.Add(newPerson);
            _context.SaveChanges();
            
            RegisterNewUserResponseDto response = _mapper.Map<RegisterNewUserResponseDto>(newPerson);
            response.Password = generatedFirstPassword;
            response.Role = nameof(AvailableRoles.TEACHER);
            return response;
        }
    }
}