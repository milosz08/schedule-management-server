using System;
using AutoMapper;

using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Services.Helpers;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class CathedralsServiceImplementation : ICathedralService
    {
        private readonly IMapper _mapper;
        private readonly ServiceHelper _helper;
        private readonly ApplicationDbContext _context; 
        
        //--------------------------------------------------------------------------------------------------------------

        public CathedralsServiceImplementation(IMapper mapper, ApplicationDbContext context, ServiceHelper helper)
        {
            _mapper = mapper;
            _context = context;
            _helper = helper;
        }
        
        //--------------------------------------------------------------------------------------------------------------        

        #region Create cathedral

        /// <summary>
        /// Metoda tworząca nową katedrę przypisaną do wybranego wydziału (na podstawie obiektu transferowego).
        /// </summary>
        /// <param name="dto">obiekt transferowy z informacjami</param>
        /// <returns>informacje o utworzonej katedrze</returns>
        /// <exception cref="BasicServerException">nieistniejący wydział/duplikat katedry w systemie</exception>
        public async Task<CathedralResponseDto> CreateCathedral(CathedralRequestDto dto)
        {
            //wyszukanie wydziału pasującego do katedry, jeśli nie znajdzie wyrzuci wyjątek
            Department findDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase));
            if (findDepartment == null) {
                throw new BasicServerException("Nie znaleziono wydziału z podanym id", HttpStatusCode.NotFound);
            }
            
            // przy próbie wprowadzeniu duplikatu katedry, wyrzuć wyjątek
            Cathedral findCathedral = await _context.Cathedrals
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => (c.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) ||
                                           c.Alias.Equals(dto.Alias, StringComparison.OrdinalIgnoreCase))
                                          && c.Department.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase));
            if (findCathedral != null) {
                throw new BasicServerException(
                    "Podana katedra istnieje już w wybranej jednostce.", HttpStatusCode.ExpectationFailed);
            }

            Cathedral cathedral = new Cathedral()
            {
                Name = dto.Name,
                Alias = dto.Alias,
                DepartmentId = findDepartment.Id,
            };

            await _context.AddAsync(cathedral);
            await _context.SaveChangesAsync();

            return _mapper.Map<CathedralResponseDto>(cathedral);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Get all cathedrals based department name

        /// <summary>
        /// Metoda odpowiedzialna za zwracanie listy wszystkich katedr z wybranego wydziału sprawdzająca wszystkie
        /// pasujące nazwy katedr (na podstawie parametru zapytania). Jeśli nie poda się parametrów, zwraca pustą listę.
        /// W przypadku braku znalezienia szukanej katedry, zwraca wszystkie pasujące do wybranego wydziału.
        /// </summary>
        /// <param name="cathName">parametr nazwy katedry</param>
        /// <param name="deptName">parametr nazwy wydziału</param>
        /// <returns>pusta lub zapełniona tablica nazwami katedr z wybranego wydziału</returns>
        public SearchQueryResponseDto GetAllCathedralsBasedDepartmentName(string cathName, string deptName)
        {
            // jeśli parametr jest nullem to przypisz wartość pustego stringa
            if (deptName == null) {
                deptName = string.Empty;
            }
            if (cathName == null) {
                cathName = string.Empty;
            }
            
            // wyszukaj i wypłaszcz rezultat do tablicy stringów z nazwami katedr
            List<string> findAllCathedralsNames = _context.Cathedrals
                .Include(c => c.Department)
                .Where(c => c.Department.Name.Equals(deptName, StringComparison.OrdinalIgnoreCase) 
                            && c.Name.Contains(cathName, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Name)
                .ToList();
            findAllCathedralsNames.Sort();
            
            if (findAllCathedralsNames.Count > 0) {
                return new SearchQueryResponseDto(findAllCathedralsNames);
            }

            List<string> findAllCathedrals = _context.Cathedrals
                .Include(c => c.Department)
                .Where(c => c.Department.Name.Equals(deptName, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Name)
                .ToList();
            findAllCathedrals.Sort();
            
            // jeśli nie znalazło pasujących rezultatów, zwróć wszystkie elementy
            return new SearchQueryResponseDto(findAllCathedrals);
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region Get all cathedrals

        /// <summary>
        /// Metoda zwracająca wszystkie katedry opakowane w obiekt paginacji i fitrowania rezultatów na podstawie
        /// przekazywanych parametrów zapytania. Umożliwia sortowanie po kolumnach (kluczach) w trybach ASC/DES.
        /// </summary>
        /// <param name="searchQuery">parametry zapytania (filtrowania wyników)</param>
        /// <returns>opakowane dane wynikowe w obiekt paginacji</returns>
        public PaginationResponseDto<CathedralQueryResponseDto> GetAllCathedrals(SearchQueryRequestDto searchQuery)
        {
            // wyszukiwanie użytkowników przy pomocy parametru SearchPhrase
            IQueryable<Cathedral> cathedralsBaseQuery = _context.Cathedrals
                .Include(c => c.Department)
                .Where(c => searchQuery.SearchPhrase == null ||
                            c.Name.Contains(searchQuery.SearchPhrase, StringComparison.OrdinalIgnoreCase));

            // sortowanie (rosnąco/malejąco) dla kolumn
            if (!string.IsNullOrEmpty(searchQuery.SortBy)) {
                _helper.PaginationSorting(new Dictionary<string, Expression<Func<Cathedral, object>>>
                {
                    { nameof(Cathedral.Id), c => c.Id },
                    { nameof(Cathedral.Name), c => c.Name },
                    { nameof(Cathedral.Alias), c => c.Alias },
                    { "DepartmentName", c => c.Department.Name },
                    { "DepartmentAlias", c => c.Department.Alias },
                }, searchQuery, ref cathedralsBaseQuery);
            }

            List<CathedralQueryResponseDto> allCathedrals = _mapper.Map<List<CathedralQueryResponseDto>>(_helper
                .PaginationAndAdditionalFiltering(cathedralsBaseQuery, searchQuery));
            
            return new PaginationResponseDto<CathedralQueryResponseDto>(
                allCathedrals, cathedralsBaseQuery.Count(), searchQuery.PageSize, searchQuery.PageNumber);
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region Get all cathedrals for selected department on schedule page
        
        /// <summary>
        ///  Metoda zwracająca wszystkie katedry na podstawie przypisanego wydziału.
        /// </summary>
        /// <param name="deptId">id wydziału do którego przypisane są katedry</param>
        /// <returns>przefiltrowane i posortowane katedry</returns>
        public List<NameWithDbIdElement> GetAllCathedralsScheduleBaseDept(long deptId)
        {
            List<Cathedral> cathedralsBaseDept = _context.Cathedrals
                .Include(s => s.Department)
                .Where(s => s.Department.Id == deptId)
                .ToList();
            cathedralsBaseDept.Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.Ordinal));
            return cathedralsBaseDept.Select(d => _mapper.Map<NameWithDbIdElement>(d)).ToList();
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Delete content

        /// <summary>
        /// Metoda usuwająca wybrane katedry z bazy danych (na podstawie wartości id w ciele zapytania).
        /// </summary>
        /// <param name="cathedrals">wszystkie numery ID elementów do usunięcia</param>
        /// <param name="credentials">obiekt autoryzacji na podstawie claimów</param>
        public async Task DeleteMassiveCathedrals(MassiveDeleteRequestDto cathedrals, UserCredentialsHeaderDto credentials)
        {
            await _helper.CheckIfUserCredentialsAreValid(credentials);
            
            // znajdowanie nieusuwalnych katedr
            IQueryable<long> nonRemovableCaths = _context.Cathedrals.Where(c => !c.IfRemovable).Select(c => c.Id);
            
            // przefiltrowanie tablicy z id wykluczając nieusuwalne katedry
            long[] filteredDeletedCathedrals = cathedrals.ElementsIds
                .Where(id => !nonRemovableCaths.Contains(id)).ToArray();

            if (filteredDeletedCathedrals.Count() > 0) {
                // filtrowanie katedr po ID znajdujących się w tablicy
                _context.Cathedrals.RemoveRange(_context.Cathedrals
                    .Where(c => filteredDeletedCathedrals.Any(id => id == c.Id)));
                await _context.SaveChangesAsync();
            }
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Metoda usuwająca z bazy danych wszystkie katedry (oprócz domyślnej zapisywanej przy seedowaniu).
        /// </summary>
        /// <param name="credentials">obiekt autoryzacji na podstawie claimów</param>
        public async Task DeleteAllCathedrals(UserCredentialsHeaderDto credentials)
        {
            await _helper.CheckIfUserCredentialsAreValid(credentials);
            
            IQueryable<Cathedral> findAllRemovingCathedrals = _context.Cathedrals.Where(c => c.IfRemovable);
            // jeśli znajdzie co najmniej jeden wydział do usunięcia
            if (findAllRemovingCathedrals.Count() > 0) {
                _context.Cathedrals.RemoveRange(findAllRemovingCathedrals);
                await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}