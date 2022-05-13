using System;
using AutoMapper;

using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Dto.Misc;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Dto.Requests;
using asp_net_po_schedule_management_server.Dto.Responses;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Ssh.SshEmailService;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public class UsersServiceImplementation : IUsersService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly ISshEmailService _emailService;
        private readonly IPasswordHasher<Person> _passwordHasher;

        //--------------------------------------------------------------------------------------------------------------
        
        public UsersServiceImplementation(
            IMapper mapper,
            ApplicationDbContext context,
            ISshEmailService emailService,
            IPasswordHasher<Person> passwordHasher)
        {
            _mapper = mapper;
            _context = context;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
        }
        
        //--------------------------------------------------------------------------------------------------------------

        #region Get all users

        // metoda zwracająca wszystkich użytkowników na podstawie wyszukiwanej frazy (oraz paginacji rezultatów)
        public PaginationResponseDto<UserResponseDto> GetAllUsers(UserQueryRequestDto query)
        {
            // wyszukiwanie użytkowników przy pomocy parametru SearchPhrase
            IQueryable<Person> usersBaseQuery = _context.Persons
                .Include(p => p.Role)
                .Where(p => query.SearchPhrase == null || p.Surname.ToLower().Contains(query.SearchPhrase.ToLower()));

            // sortowanie (rosnąco/malejąco) dla kolumn
            if (!string.IsNullOrEmpty(query.SortBy)) {
                var colSelectors = new Dictionary<string, Expression<Func<Person, object>>>
                {
                    { nameof(Person.Id), p => p.Id },
                    { nameof(Person.Surname), p => p.Surname },
                    { nameof(Person.Login), p => p.Login },
                    { nameof(Person.Role), p => p.Role.Name },
                };

                Expression<Func<Person,object>> selectColumn = colSelectors[query.SortBy];
                
                usersBaseQuery = query.SortDirection == SortDirection.ASC
                    ? usersBaseQuery.OrderBy(selectColumn)
                    : usersBaseQuery.OrderByDescending(selectColumn);
            }
            
            // paginacja i dodatkowe filtrowanie
            List<Person> findAllUsers = usersBaseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();

            List<UserResponseDto> allUsers = _mapper.Map<List<UserResponseDto>>(findAllUsers);
            return new PaginationResponseDto<UserResponseDto>(
                allUsers, usersBaseQuery.Count(), query.PageSize, query.PageNumber);
        }
        
        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region Delete massive users

        // usuwanie masywne (na podstawie identyfikatorów z tablicy)
        public async Task DeleteMassiveUsers(MassiveDeleteRequestDto deleteUsers, UserCredentialsHeaderDto credentials)
        {
            await CheckIfUserCredentialsAreValid(credentials);
            
            // znajdowanie osób z nieusuwalnym kontem
            IQueryable<long> personsNotRemoveAccounts = _context.Persons
                .Where(p => !p.IfRemovable)
                .Select(p => p.Id);

            // przefiltrowanie tablicy z id wykluczając użytkowników z nieusuwalnymi kontami
            long[] filteredDeletedPersons = deleteUsers.ElementsIds
                .Where(id => !personsNotRemoveAccounts.Contains(id)).ToArray();

            if (filteredDeletedPersons.Count() > 0) {
                // filtrowanie użytkowników po ID znajdujących się w tablicy
                IQueryable<Person> personsToRemove = _context.Persons
                    .Where(p => filteredDeletedPersons.Any(id => id == p.Id));

                // usuwanie skrzynek email
                foreach (Person person in personsToRemove) {
                    _emailService.DeleteEmailAccount(person.Email);
                }
                
                _context.Persons.RemoveRange(personsToRemove);
                await _context.SaveChangesAsync();
            }
        }
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Delete all users

        // usuwanie wszystkich użytkowników z bazy danych (oprócz użytkownika domyślnego)
        public async Task DeleteAllUsers(UserCredentialsHeaderDto credentials)
        {
            IQueryable<Person> findAllRemovingPersons = _context.Persons.Where(p => !p.IfRemovable);
            if (findAllRemovingPersons.Count() > 0) {
                // usuwanie skrzynek email
                foreach (Person person in findAllRemovingPersons) {
                    _emailService.DeleteEmailAccount(person.Email);
                }
                _context.Persons.RemoveRange(findAllRemovingPersons);
                await _context.SaveChangesAsync();
            }
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Helper methods

        /// <summary>
        /// Metoda walidująca użytkownika na podstawie wprowadzonego hasła oraz loginu pozyskanego z tokenu JWT
        /// oraz podanego przez użytkownika.
        /// </summary>
        /// <param name="credentials">obiekt przechowujący wartości autoryzacji</param>
        /// <returns>Zwróci true, jeśli autoryzacja przebiegła prawidłowo.</returns>
        /// <exception cref="BasicServerException">W przypadku błędu serwera wyrzuci wyjątek</exception>
        private async Task CheckIfUserCredentialsAreValid(UserCredentialsHeaderDto credentials)
        {
            Person findPerson = await _context.Persons
                .FirstOrDefaultAsync(p => p.Login == credentials.Login && p.Login == credentials.Username);

            // jeśli użytkownik nie istnieje w systemie
            if (findPerson == null) {
                throw new BasicServerException("Użytkownik z podanym loginem/nazwą nie istnieje w systemie.", 
                    HttpStatusCode.Forbidden);
            }
            
            // weryfikacja hasła
            PasswordVerificationResult verificatrionRes = _passwordHasher
                .VerifyHashedPassword(findPerson, findPerson.Password, credentials.Password);
            if (verificatrionRes == PasswordVerificationResult.Failed) {
                throw new BasicServerException("Nieprawidłowe hasło. Spróbuj ponownie.", HttpStatusCode.Unauthorized);
            }
        }

        #endregion
    }
}