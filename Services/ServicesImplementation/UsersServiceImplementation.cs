using AutoMapper;

using System;
using System.Net;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Exceptions;
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
        /// Metoda zwracająca wszystkich pracowników (wszyscy użytkownicy oprócz studentów) na podstawie wybranego
        /// wydziału oraz katedry.
        /// </summary>
        /// <param name="deptId">id wydziału</param>
        /// <param name="cathId">id katedry</param>
        /// <returns>przefiltrowane oraz posortowane wyniki w postaci listy pracowników</returns>
        public async Task<List<NameWithDbIdElement>> GetAllEmployeersScheduleBaseCath(long deptId, long cathId)
        {
            List<Person> allUsersWithoutStudents = await _context.Persons
                .Include(p => p.Role)
                .Include(p => p.Department)
                .Include(p => p.Cathedral)
                .Where(p => p.Department.Id == deptId && p.Cathedral.Id == cathId &&
                            !p.Role.Name.Equals(AvailableRoles.STUDENT, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
            
            allUsersWithoutStudents
                .Sort((first, second) => string.Compare(first.Surname, second.Surname, StringComparison.Ordinal));
            return allUsersWithoutStudents.Select(d => new NameWithDbIdElement(d.Id, $"{d.Surname} {d.Name}")).ToList();
        }
        
        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Get all teachers base department id

        /// <summary>
        /// Metoda zwracająca wszystkich nauczycieli (wszyscy użytkownicy oprócz studentów) na podstawie wybranego
        /// wydziału oraz przypisanego (wybranego w parametrach) przedmiotu.
        /// </summary>
        /// <param name="deptId">id wydziału</param>
        /// <param name="subjName">nazwa przypisanego przedmiotu</param>
        /// <returns>przefiltrowane oraz posortowane wyniki w postaci listy nauczycieli</returns>
        public async Task<List<NameWithDbIdElement>> GetAllTeachersScheduleBaseDeptAndSpec(long deptId, string subjName)
        {
            List<Person> selectedUsersWithoutStudents = await _context.Persons
                .Include(p => p.Role)
                .Include(p => p.Department)
                .Include(p => p.Subjects)
                .Where(p => p.Department.Id == deptId &&
                            p.Subjects.Any(s => s.Name.Equals(subjName, StringComparison.OrdinalIgnoreCase))
                            && !p.Role.Name.Equals(AvailableRoles.STUDENT, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            if (selectedUsersWithoutStudents.Count > 0) {
                selectedUsersWithoutStudents
                    .Sort((first, second) => string.Compare(first.Surname, second.Surname, StringComparison.Ordinal));
                return selectedUsersWithoutStudents.Select(d => new NameWithDbIdElement(d.Id, $"{d.Surname} {d.Name}")).ToList();  
            }
            
            List<Person> allUsersWithoutStudents = await _context.Persons
                .Include(p => p.Role)
                .Include(p => p.Department)
                .Where(p => p.Department.Id == deptId && 
                            !p.Role.Name.Equals(AvailableRoles.STUDENT, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
            
            allUsersWithoutStudents
                .Sort((first, second) => string.Compare(first.Surname, second.Surname, StringComparison.Ordinal));
            return allUsersWithoutStudents.Select(d => new NameWithDbIdElement(d.Id, $"{d.Surname} {d.Name}")).ToList();  
        }
        
        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region User dashboard panel properties

        /// <summary>
        /// Metoda pobierająca szczegółowe dane użytkownika z bazyd danych i zwracająca je w postaci obiektu
        /// transferowego. Metoda pobiera dane na podstawie identyfikacji JWT. Jeśli token jest nieprawidłowy, lub jeśli
        /// nie znajdzie użytkownika z odszyfrowanymi danymi logowania, wyrzuci wyjątek.
        /// </summary>
        /// <param name="userIdentity">claimy identyfikujące użytkownika</param>
        /// <returns>dane do głównego panelu konta opakowane w obiekt transferowy</returns>
        /// <exception cref="BasicServerException">błędy JWT/brak JWT/nieautoryzowany dostęp do zasobu</exception>
        public async Task<DashboardDetailsResDto> GetDashboardPanelData(Claim userIdentity)
        {
            // jeśli JWT jest nieprawidłowy, rzuć wyjątek dostępu (forbidden 403)
            if (userIdentity == null) {
                throw new BasicServerException("Dostęp do zasobu zabroniony.", HttpStatusCode.Forbidden);
            }
            // wyszukaj osobę na podstawie loginu z JWT, jeśli nie znajdzie rzuć wyjątek 404
            Person findPerson = await _context.Persons
                .Include(p => p.Role)
                .Include(p => p.Subjects)
                .Include(p => p.Cathedral)
                .Include(p => p.Department)
                .Include(p => p.StudySpecializations)
                .FirstOrDefaultAsync(p => p.Login == userIdentity.Value);
            if (findPerson == null) {
                throw new BasicServerException("Podany użytkownik nie istenieje w systemie.", HttpStatusCode.NotFound);
            }
            
            DashboardDetailsResDto dashboardDetailsResDto = _mapper.Map<DashboardDetailsResDto>(findPerson);

            if (findPerson.Cathedral != null) {
                dashboardDetailsResDto.CathedralFullName = $"{findPerson.Cathedral.Name} ({findPerson.Cathedral.Alias})";
            }

            if (findPerson.Role.Name == AvailableRoles.STUDENT) {
                dashboardDetailsResDto.StudySpecializations =
                    findPerson.StudySpecializations.Select(s => $"{s.Name} ({s.Alias})").ToList();
            }

            if (findPerson.Role.Name == AvailableRoles.TEACHER || findPerson.Role.Name == AvailableRoles.EDITOR) {
                dashboardDetailsResDto.StudySubjects = findPerson.Subjects.Select(s => $"{s.Name} ({s.Alias})").ToList();
            }

            if (findPerson.Role.Name == AvailableRoles.ADMINISTRATOR) {
                dashboardDetailsResDto.DashboardElementsCount = new DashboardElementsCount(
                    _context.Departments.Count(),
                    _context.Cathedrals.Count(),
                    _context.StudyRooms.Count(),
                    _context.StudySpecializations.Count(),
                    _context.StudySubjects.Count(),
                    _context.StudyGroups.Count()
                );
                dashboardDetailsResDto.DashboardUserTypesCount = new DashboardUserTypesCount(
                    _context.Persons.Include(r => r.Role).Where(p => p.Role.Name == AvailableRoles.STUDENT).Count(),
                    _context.Persons.Include(r => r.Role).Where(p => p.Role.Name == AvailableRoles.TEACHER).Count(),
                    _context.Persons.Include(r => r.Role).Where(p => p.Role.Name == AvailableRoles.EDITOR).Count(),
                    _context.Persons.Include(r => r.Role).Where(p => p.Role.Name == AvailableRoles.ADMINISTRATOR).Count()
                );
            }
            
            return dashboardDetailsResDto;
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