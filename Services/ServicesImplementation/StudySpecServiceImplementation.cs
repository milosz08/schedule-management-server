using System;
using AutoMapper;

using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Services.Helpers;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public class StudySpecServiceImplementation : IStudySpecService
    {
        private readonly IMapper _mapper;
        private readonly ServiceHelper _helper;
        private readonly ApplicationDbContext _context;

        //--------------------------------------------------------------------------------------------------------------

        public StudySpecServiceImplementation(ServiceHelper helper, IMapper mapper, ApplicationDbContext context)
        {
            _helper = helper;
            _mapper = mapper;
            _context = context;
        }

        //--------------------------------------------------------------------------------------------------------------

        #region Add new study specialization

        /// <summary>
        /// Metoda odpowiedzialna za dodawanie nowych kierunków studiów. Metoda sprawdza, czy kierunek istnieje w
        /// systemie oraz czy powiązany z nim wydział również istnieje. Metoda umożliwa stworzenie jednocześnie dwóch
        /// kierunków studiów (dzienne/zaoczne) jeśli taki parametr znajduje się w obiekcie transferowym.
        /// </summary>
        /// <param name="dto">obiekt transferowy z danymi odnośnie nowego kierunku studiów</param>
        /// <returns>utworzone kierunek/kierunki studiów</returns>
        /// <exception cref="BasicServerException">nieistniejący wydział/duplikat kierunku/brak typu kierunku</exception>
        public async Task<IEnumerable<CreateStudySpecResponseDto>> AddNewStudySpecialization(CreateStudySpecRequestDto dto)
        {
            //wyszukanie wydziału pasującego do kierunku, jeśli nie znajdzie wyrzuci wyjątek
            Department findDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase));
            if (findDepartment == null) {
                throw new BasicServerException("Nie znaleziono wydziału z podaną nazwą", HttpStatusCode.NotFound);
            }

            List<StudyType> findAllStudyTypes = _context.StudyTypes
                .Where(t => dto.StudyType.Any(id => id == t.Id)).ToList();
            if (findAllStudyTypes.Count == 0) {
                throw new BasicServerException("Nie znaleziono podanych id typów kierunków", HttpStatusCode.NotFound);
            }

            List<StudyDegree> findAllStudyDegrees = _context.StudyDegrees
                .Where(d => dto.StudyDegree.Any(id => id == d.Id)).ToList();
            if (findAllStudyDegrees.Count == 0) {
                throw new BasicServerException("Nie znaleziono podanych id stopni studiów", HttpStatusCode.NotFound);
            }
            
            // przy próbie wprowadzeniu duplikatu kierunku studiów, wyrzuć wyjątek
            StudySpecialization findSpecialization = await _context.StudySpecializations
                .Include(s => s.Department)
                .Include(s => s.StudyType)
                .Include(s => s.StudyDegree)
                .FirstOrDefaultAsync(s => (s.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) ||
                                           s.Alias.Equals(dto.Alias, StringComparison.OrdinalIgnoreCase)) &&
                                          s.Department.Name.Equals(dto.DepartmentName, StringComparison.Ordinal) &&
                                          dto.StudyType.Any(v => v == s.StudyType.Id) &&
                                          dto.StudyDegree.Any(v => v == s.StudyDegree.Id));
            
            if (findSpecialization != null) {
                throw new BasicServerException(
                    "Podany kierunek studiów istnieje już w wybranej jednostce.", HttpStatusCode.ExpectationFailed);
            }
            
            List<StudySpecialization> createdSpecializations = new List<StudySpecialization>();

            foreach (StudyType studyType in findAllStudyTypes) {
                foreach (StudyDegree studyDegree in findAllStudyDegrees) {
                    createdSpecializations.Add(new StudySpecialization()
                    {
                        Name = dto.Name,
                        Alias = dto.Alias,
                        DepartmentId = findDepartment.Id,
                        StudyTypeId = studyType.Id,
                        StudyDegreeId = studyDegree.Id,
                    });
                }
            }

            // zapis do bazy danych
            await _context.StudySpecializations.AddRangeAsync(createdSpecializations);
            await _context.SaveChangesAsync();
            return createdSpecializations.Select(s => _mapper.Map<CreateStudySpecResponseDto>(s));
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region Get all available study types based department

        /// <summary>
        /// 
        /// </summary>
        /// <param name="specName"></param>
        /// <param name="deptName"></param>
        /// <returns></returns>
        public SearchQueryResponseDto GetAllStudySpecializationsInDepartment(string specName, string deptName)
        {
            // jeśli parametr jest nullem to przypisz wartość pustego stringa
            if (deptName == null) {
                deptName = string.Empty;
            }
            if (specName == null) {
                specName = string.Empty;
            }
            
            // wyszukaj i wypłaszcz rezultat do tablicy stringów z nazwami katedr
            List<string> findAllStudySpecializations = _context.StudySpecializations
                .Include(s => s.Department)
                .Include(s => s.StudyType)
                .Include(s => s.StudyDegree)
                .Where(s => s.Department.Name.Equals(deptName, StringComparison.OrdinalIgnoreCase) 
                            && s.Name.Contains(specName, StringComparison.OrdinalIgnoreCase))
                .Select(s => $"{s.Name} ({s.StudyType.Alias} {s.StudyDegree.Alias})")
                .ToList();
            findAllStudySpecializations.Sort();
            
            if (findAllStudySpecializations.Count > 0) {
                return new SearchQueryResponseDto(findAllStudySpecializations);
            }

            List<string> findAllElements = _context.StudySpecializations
                .Include(s => s.Department)
                .Include(s => s.StudyType)
                .Include(s => s.StudyDegree)
                .Where(s => s.Department.Name.Equals(deptName, StringComparison.OrdinalIgnoreCase))
                .Select(s => $"{s.Name} ({s.StudyType.Alias} {s.StudyDegree.Alias}")
                .ToList();
            findAllElements.Sort();
            
            // jeśli nie znalazło pasujących rezultatów, zwróć wszystkie elementy
            return new SearchQueryResponseDto(findAllStudySpecializations);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Get all study specializations

        /// <summary>
        /// Metoda zwracająca wszystkie kierunku studiów opakowane w obiekt paginacji i fitrowania rezultatów na podstawie
        /// przekazywanych parametrów zapytania. Umożliwia sortowanie po kolumnach (kluczach) w trybach ASC/DES.
        /// </summary>
        /// <param name="searchQuery">parametry zapytania (filtrowania wyników)</param>
        /// <returns>opakowane dane wynikowe w obiekt paginacji</returns>
        public PaginationResponseDto<StudySpecQueryResponseDto> GetAllStudySpecializations(SearchQueryRequestDto searchQuery)
        {
            // wyszukiwanie użytkowników przy pomocy parametru SearchPhrase
            IQueryable<StudySpecialization> studySpecsBaseQuery = _context.StudySpecializations
                .Include(s => s.Department)
                .Include(s => s.StudyType)
                .Include(s => s.StudyDegree)
                .Where(s => searchQuery.SearchPhrase == null ||
                            s.Name.Contains(searchQuery.SearchPhrase, StringComparison.OrdinalIgnoreCase));

            // sortowanie (rosnąco/malejąco) dla kolumn
            if (!string.IsNullOrEmpty(searchQuery.SortBy)) {
                _helper.PaginationSorting(new Dictionary<string, Expression<Func<StudySpecialization, object>>>
                {
                    { nameof(StudySpecialization.Id), s => s.Id },
                    { nameof(StudySpecialization.Name), s => s.Name },
                    { "DepartmentAlias", s => s.Department.Alias },
                    { "SpecTypeAlias", s => s.StudyType.Alias },
                    { "SpecDegree", s => s.StudyDegree.Alias },
                }, searchQuery, ref studySpecsBaseQuery);
            }
            
            List<StudySpecQueryResponseDto> allDepts = _mapper.Map<List<StudySpecQueryResponseDto>>(_helper
                .PaginationAndAdditionalFiltering(studySpecsBaseQuery, searchQuery));
            
            return new PaginationResponseDto<StudySpecQueryResponseDto>(
                allDepts, studySpecsBaseQuery.Count(), searchQuery.PageSize, searchQuery.PageNumber);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Get all study specializations for selected department on schedule page
        
        /// <summary>
        /// Metoda zwracająca wszystkie kierunku studiów w postaci tupli (name, id) na podstawie id wydziału. Używana
        /// dla punktu końcowego niechronionego, przy wyświetlaniu planu.
        /// </summary>
        /// <returns>wszystkie znalezione kierunki studiów</returns>
        public async Task<List<NameWithDbIdElement>> GetAllStudySpecsScheduleBaseDept(long deptId, long degreeId)
        {
            List<StudySpecialization> studySpecsBaseDept = await _context.StudySpecializations
                .Include(s => s.Department)
                .Include(s => s.StudyType)
                .Include(s => s.StudyDegree)
                .Where(s => s.Department.Id == deptId && s.StudyDegree.Id == degreeId)
                .ToListAsync();
            studySpecsBaseDept.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.Ordinal));
            return studySpecsBaseDept.Select(d => new NameWithDbIdElement(d.Id, $"{d.Name} ({d.StudyType.Alias})")).ToList();
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Get available study specializations base department name

        /// <summary>
        /// Metoda pobierająca i zwracająca wszystkie dostępne kierunki studiów z bazy danych w postaci tupli (nazwa, id)
        /// na podstawie nazwy wydziału. Metoda nie posiada dynamicznego filtrowania wyników, zwraca jedynie statyczne
        /// dane pozyskane z bazy danych.
        /// </summary>
        /// <param name="deptName">nazwa wydziału</param>
        /// <returns>zwracane wyniki opakowane w obiekt transferowy</returns>
        public async Task<AvailableDataResponseDto<NameWithDbIdElement>> GetAvailableStudySpecsBaseDept(string deptName)
        {
            List<StudySpecialization> findAllStudySpecs = await _context.StudySpecializations
                .Include(s => s.Department)
                .Include(s => s.StudyType)
                .Include(s => s.StudyDegree)
                .Where(s => s.Department.Name.Equals(deptName, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            return new AvailableDataResponseDto<NameWithDbIdElement>(findAllStudySpecs
                .Select(s => _mapper.Map<NameWithDbIdElement>(s)).ToList());
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Delete content

        /// <summary>
        /// Metoda usuwająca wybrane kierunki studiów z bazy danych (na podstawie wartości id w ciele zapytania).
        /// </summary>
        /// <param name="studySpecs">wszystkie numery ID elementów do usunięcia</param>
        /// <param name="credentials">obiekt autoryzacji na podstawie claimów</param>
        public async Task DeleteMassiveStudySpecs(MassiveDeleteRequestDto studySpecs, UserCredentialsHeaderDto credentials)
        {
            await _helper.CheckIfUserCredentialsAreValid(credentials);
            // filtrowanie kierunków studiów po ID znajdujących się w tablicy
            _context.StudySpecializations.RemoveRange(_context.StudySpecializations
                .Where(s => studySpecs.ElementsIds.Any(id => id == s.Id)));
            await _context.SaveChangesAsync();
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Metoda usuwająca z bazy danych wszystkie kierunki studiów.
        /// </summary>
        /// <param name="credentials">obiekt autoryzacji na podstawie claimów</param>
        public async Task DeleteAllStudySpecs(UserCredentialsHeaderDto credentials)
        {
            await _helper.CheckIfUserCredentialsAreValid(credentials);
            _context.StudySpecializations.RemoveRange();
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}