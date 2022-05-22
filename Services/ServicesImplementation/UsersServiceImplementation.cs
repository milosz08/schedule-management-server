using AutoMapper;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Services.Helpers;
using asp_net_po_schedule_management_server.Ssh.SshEmailService;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class UsersServiceImplementation : IUsersService
    {
        private readonly IMapper _mapper;
        private readonly ServiceHelper _helper;
        private readonly ApplicationDbContext _context;
        private readonly ISshEmailService _emailService;

        //--------------------------------------------------------------------------------------------------------------
        
        public UsersServiceImplementation(IMapper mapper, ApplicationDbContext context, ServiceHelper helper,
            ISshEmailService emailService)
        {
            _mapper = mapper;
            _helper = helper;
            _context = context;
            _emailService = emailService;
        }
        
        //--------------------------------------------------------------------------------------------------------------

        #region Get all users

        /// <summary>
        /// Metoda zwracająca wszystkich użytkowników na podstawie wyszukiwanej frazy (oraz paginacji rezultatów).
        /// </summary>
        /// <param name="searchQuery">zapytanie w postaci obiektu przechowującego wszystkie parametry sortowania</param>
        /// <returns>właściwości paginacji rezultatu (ilość stron, wyników na stronę itp.)</returns>
        public PaginationResponseDto<UserResponseDto> GetAllUsers(SearchQueryRequestDto searchQuery)
        {
            // wyszukiwanie użytkowników przy pomocy parametru SearchPhrase
            IQueryable<Person> usersBaseQuery = _context.Persons
                .Include(p => p.Role)
                .Where(p => searchQuery.SearchPhrase == null ||
                            p.Surname.Contains(searchQuery.SearchPhrase, StringComparison.OrdinalIgnoreCase));

            // sortowanie (rosnąco/malejąco) dla kolumn
            if (!string.IsNullOrEmpty(searchQuery.SortBy)) {
                _helper.PaginationSorting(new Dictionary<string, Expression<Func<Person, object>>>
                {
                    { nameof(Person.Id), p => p.Id },
                    { nameof(Person.Surname), p => p.Surname },
                    { nameof(Person.Login), p => p.Login },
                    { nameof(Person.Role), p => p.Role.Name },
                }, searchQuery, ref usersBaseQuery);
            }

            List<UserResponseDto> allUsers = _mapper.Map<List<UserResponseDto>>(_helper
                .PaginationAndAdditionalFiltering(usersBaseQuery, searchQuery));
            
            return new PaginationResponseDto<UserResponseDto>(
                allUsers, usersBaseQuery.Count(), searchQuery.PageSize, searchQuery.PageNumber);
        }
        
        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Get all employeers base cathedral database id

        /// <summary>
        /// Metoda zwracająca wszystkich pracowników (wszystcy użytkownicy oprócz studentów) na podstawie wybranego
        /// wydziału oraz katedry.
        /// </summary>
        /// <param name="deptId">id wydziału</param>
        /// <param name="cathId">id katedry</param>
        /// <returns>przefiltrowane oraz posortowane wyniki w postaci listy pracowników</returns>
        public List<NameWithDbIdElement> GetAllEmployeersScheduleBaseCath(long deptId, long cathId)
        {
            List<Person> allUsersWithoutStudents = _context.Persons
                .Include(p => p.Role)
                .Include(p => p.Department)
                .Include(p => p.Cathedral)
                .Where(p => p.Department.Id == deptId && p.Cathedral.Id == cathId &&
                            p.Role.Name.ToLower() != AvailableRoles.STUDENT.ToLower()).ToList();
            
            allUsersWithoutStudents
                .Sort((first, second) => string.Compare(first.Surname, second.Surname, StringComparison.Ordinal));
            return allUsersWithoutStudents.Select(d => _mapper.Map<NameWithDbIdElement>(d)).ToList();
        }
        
        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region Delete massive users

        /// <summary>
        /// Usuwanie masywne (na podstawie identyfikatorów z tablicy).
        /// </summary>
        /// <param name="users">tablica z id użytkowników do usunięcia</param>
        /// <param name="credentials">obiekt przechowujący dane autentykacji operacji na danych</param>
        public async Task DeleteMassiveUsers(MassiveDeleteRequestDto users, UserCredentialsHeaderDto credentials)
        {
            await _helper.CheckIfUserCredentialsAreValid(credentials);
            
            // znajdowanie osób z nieusuwalnym kontem
            IQueryable<long> personsNotRemoveAccounts = _context.Persons.Where(p => !p.IfRemovable).Select(p => p.Id);

            // przefiltrowanie tablicy z id wykluczając użytkowników z nieusuwalnymi kontami
            long[] filteredDeletedPersons = users.ElementsIds
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

        /// <summary>
        /// Usuwanie wszystkich użytkowników z bazy danych (oprócz użytkownika domyślnego).
        /// </summary>
        /// <param name="credentials">obiekt przechowujący dane autentykacji operacji na danych</param>
        public async Task DeleteAllUsers(UserCredentialsHeaderDto credentials)
        {
            IQueryable<Person> findAllRemovingPersons = _context.Persons.Where(p => p.IfRemovable);
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
    }
}