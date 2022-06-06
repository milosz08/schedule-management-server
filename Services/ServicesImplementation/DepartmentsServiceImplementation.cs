using System;
using AutoMapper;

using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Services.Helpers;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class DepartmentsServiceImplementation : IDepartmentsService
    {
        private readonly IMapper _mapper;
        private readonly ServiceHelper _helper;
        private readonly ApplicationDbContext _context;

        //--------------------------------------------------------------------------------------------------------------

        public DepartmentsServiceImplementation(IMapper mapper, ApplicationDbContext context, ServiceHelper helper)
        {
            _mapper = mapper;
            _context = context;
            _helper = helper;
        }

        //--------------------------------------------------------------------------------------------------------------        

        #region Create department

        /// <summary>
        /// Metoda odpowiadająca za dodawanie nowego wydziału do bazy danych. Metoda sprawdza, czy nie dochodzi do
        /// dodania duplikatu; jeśli tak, wyrzuca wyjątek.
        /// </summary>
        /// <param name="dto">dataobject przechowujący dane wydziału</param>
        /// <returns>obiekt z informacjami o stworzonym wydziale</returns>
        /// <exception cref="BasicServerException">jeśli wykryje duplikat wydziału w tabeli</exception>
        public async Task<DepartmentRequestResponseDto> CreateDepartment(DepartmentRequestResponseDto dto)
        {
            // przy próbie wprowadzeniu duplikatu wydziału, wyrzuć wyjątek
            Department findDepartment = await _context.Departments.FirstOrDefaultAsync(d =>
                d.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) ||
                d.Alias.Equals(dto.Alias, StringComparison.OrdinalIgnoreCase));
            if (findDepartment != null) {
                throw new BasicServerException("Podany wydział istnieje już w systemie.", HttpStatusCode.ExpectationFailed);
            }

            // mapowanie obiektu DTO na instancję encji dodawaną do bazy danych
            Department newDepartment = _mapper.Map<Department>(dto);
            await _context.Departments.AddAsync(newDepartment);
            await _context.SaveChangesAsync();

            return dto;
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Update department

        /// <summary>
        /// Metoda odpowiedzialna za aktualizowanie danych wybranego wydziału (na podstawie ciała zapytania i parametrów).
        /// </summary>
        /// <param name="dto">obiekt z danymi do zamiany</param>
        /// <param name="deptId">id wydziału podlegającego zamianie</param>
        /// <returns>zamienione dane w postaci obiektu transferowego</returns>
        /// <exception cref="BasicServerException">jeśli nie znajdzie wydziału/próba wprowadzenia tych samych danych</exception>
        public async Task<DepartmentRequestResponseDto> UpdateDepartment(DepartmentRequestResponseDto dto, long deptId)
        {
            // wyszukaj wydział na podstawie id, jeśli nie znajdzie rzuć wyjątek
            Department findDepartment = await _context.Departments.FirstOrDefaultAsync(d => d.Id == deptId);
            if (findDepartment == null) {
                throw new BasicServerException("Nie znaleziono wydziału z podanym id.", HttpStatusCode.NotFound);
            }
            // sprawdź czy dane dotyczące wydziału są zmieniane
            if (findDepartment.Name == dto.Name && findDepartment.Alias == dto.Alias) {
                throw new BasicServerException("Należy wprowadzić wartości różne od poprzednich.", 
                    HttpStatusCode.ExpectationFailed);
            }
            
            findDepartment.Name = dto.Name;
            findDepartment.Alias = dto.Alias;
            await _context.SaveChangesAsync();
            
            return dto;
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Get all departments list

        /// <summary>
        /// Metoda filtrująca (na podstawie głębokiego zapytania) i zwracająca listę wszystkich wydziałów
        /// (spłaszczone dane w formie listy stringów).
        /// </summary>
        /// <param name="deptQuerySearch">parametr zapytania służący do filtrowania wyników</param>
        /// <returns>wszystkie wydziały</returns>
        public SearchQueryResponseDto GetAllDepartmentsList(string deptQueryName)
        {
            if (deptQueryName == null || deptQueryName == string.Empty) {
                List<string> allDepartments = _context.Departments
                    .Select(d => d.Name)
                    .ToList();
                allDepartments.Sort();
                
                return new SearchQueryResponseDto(allDepartments);
            }

            // spłaszczanie i sortowanie wyniku pobrania wszystkich wydziałów na podstawie parametru wyszukiwania
            List<string> findAllDepartments = _context.Departments
                .Where(d => d.Name.Contains(deptQueryName, StringComparison.OrdinalIgnoreCase))
                .Select(d => d.Name)
                .ToList();
            findAllDepartments.Sort();
            
            if (findAllDepartments.Count > 0) {
                return new SearchQueryResponseDto(findAllDepartments);
            }
            
            return new SearchQueryResponseDto();
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Get all departments
        
        /// <summary>
        /// Metoda zwracająca wszystkie wydziały opakowane w obiekt paginacji i fitrowania rezultatów na podstawie
        /// przekazywanych parametrów zapytania. Umożliwia sortowanie po kolumnach (kluczach) w trybach ASC/DES.
        /// </summary>
        /// <param name="searchQuery">parametry zapytania (filtrowania wyników)</param>
        /// <returns>opakowane dane wynikowe w obiekt paginacji</returns>
        public PaginationResponseDto<DepartmentQueryResponseDto> GetAllDepartments(SearchQueryRequestDto searchQuery)
        {
            // wyszukiwanie wydziałów przy pomocy parametru SearchPhrase
            IQueryable<Department> deparmentsBaseQuery = _context.Departments
                .Where(d => searchQuery.SearchPhrase == null ||
                            d.Name.Contains(searchQuery.SearchPhrase, StringComparison.OrdinalIgnoreCase));

            // sortowanie (rosnąco/malejąco) dla kolumn
            if (!string.IsNullOrEmpty(searchQuery.SortBy)) {
                _helper.PaginationSorting(new Dictionary<string, Expression<Func<Department, object>>>
                {
                    { nameof(Department.Id), d => d.Id },
                    { nameof(Department.Name), d => d.Name },
                    { nameof(Department.Alias), d => d.Alias },
                }, searchQuery, ref deparmentsBaseQuery);
            }
            
            List<DepartmentQueryResponseDto> allDepts = _mapper.Map<List<DepartmentQueryResponseDto>>(_helper
                .PaginationAndAdditionalFiltering(deparmentsBaseQuery, searchQuery));
            
            return new PaginationResponseDto<DepartmentQueryResponseDto>(
                allDepts, deparmentsBaseQuery.Count(), searchQuery.PageSize, searchQuery.PageNumber);
        }
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Get all departments for schedule page
        
        /// <summary>
        /// Metoda zwracająca wszystkie wydziały w postaci tupli (name, id). Używana dla punktu końcowego niechronionego,
        /// przy wyświetlaniu planu.
        /// </summary>
        /// <returns>wszystkie znalezione wydziały</returns>
        public async Task<List<NameWithDbIdElement>> GetAllDepartmentsSchedule()
        {
            List<Department> findAllDepartments = await _context.Departments
                .Select(d => d).ToListAsync();
            findAllDepartments.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.Ordinal));
            return findAllDepartments.Select(d => _mapper.Map<NameWithDbIdElement>(d)).ToList();
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region Get department data base department id

        /// <summary>
        /// Metoda pobierająca zawartość wydziału z bazy danych na podstawie przekazywanego parametru id w parametrach
        /// zapytania HTTP. Metoda używana głównie w celu aktualizacji istniejących treści w serwisie.
        /// </summary>
        /// <param name="deptId">id wydziału</param>
        /// <returns>obiekt transferowy z danymi konkretnego wydziału</returns>
        /// <exception cref="BasicServerException">w przypadku nieznalezienia wydziału z podanym id</exception>
        public async Task<DepartmentEditResDto> GetDepartmentBaseDbId(long deptId)
        {
            // wyszukaj wydział na podstawie parametru ID w bazie danych, jeśli nie znajdzie rzuć 404.
            Department findDepartment = await _context.Departments.FirstOrDefaultAsync(d => d.Id == deptId);
            if (findDepartment == null) {
                throw new BasicServerException("Nie znaleziono wydziału z podanym numerem id.", HttpStatusCode.NotFound);
            }

            return _mapper.Map<DepartmentEditResDto>(findDepartment);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Delete massive

        /// <summary>
        /// Metoda usuwająca wybrane wydziały z bazy danych (na podstawie wartości id w ciele zapytania).
        /// </summary>
        /// <param name="departments">wszystkie numery ID elementów do usunięcia</param>
        /// <param name="credentials">obiekt autoryzacji na podstawie claimów</param>
        public async Task DeleteMassiveDepartments(MassiveDeleteRequestDto departments, UserCredentialsHeaderDto credentials)
        {
            // sprawdź, czy usunięcie jest realizowane z konta administratora, jeśli nie wyrzuć wyjątek
            if (credentials.Person.Role.Name != AvailableRoles.ADMINISTRATOR) {
                throw new BasicServerException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora.",
                    HttpStatusCode.Forbidden);
            }
            
            // znajdowanie nieusuwalnych wydziałów
            IQueryable<long> nonRemovableDepts = _context.Departments.Where(d => !d.IfRemovable).Select(d => d.Id);
            
            // przefiltrowanie tablicy z id wykluczając nieusuwalne wydziały
            long[] filteredDeletedDepartments = departments.ElementsIds
                .Where(id => !nonRemovableDepts.Contains(id)).ToArray();

            if (filteredDeletedDepartments.Count() > 0) {
                // filtrowanie wydziałów po ID znajdujących się w tablicy
                _context.Departments.RemoveRange(_context.Departments
                    .Where(d => filteredDeletedDepartments.Any(id => id == d.Id)));
                await _context.SaveChangesAsync();
            }
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Delete all

        /// <summary>
        /// Metoda usuwająca z bazy danych wszystkie wydziały (oprócz domyślnego zapisywanego przy seedowaniu).
        /// </summary>
        /// <param name="credentials">obiekt autoryzacji na podstawie claimów</param>
        public async Task DeleteAllDepartments(UserCredentialsHeaderDto credentials)
        {
            // sprawdź, czy usunięcie jest realizowane z konta administratora, jeśli nie wyrzuć wyjątek
            if (credentials.Person.Role.Name != AvailableRoles.ADMINISTRATOR) {
                throw new BasicServerException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora.",
                    HttpStatusCode.Forbidden);
            }
            
            IQueryable<Department> findAllRemovingDepartments = _context.Departments.Where(d => d.IfRemovable);
            // jeśli znajdzie co najmniej jeden wydział do usunięcia
            if (findAllRemovingDepartments.Count() > 0) {
                _context.Departments.RemoveRange(findAllRemovingDepartments);
                await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}