using AutoMapper;

using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Dto.RequestResponseMerged;
using asp_net_po_schedule_management_server.Dto.Responses;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class CathedralsServiceImplementation : ICathedralService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context; 
        
        //--------------------------------------------------------------------------------------------------------------

        public CathedralsServiceImplementation(IMapper mapper, ApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        
        //--------------------------------------------------------------------------------------------------------------        

        #region Create cathedral

        /// <summary>
        /// Metoda tworząca nową katedrę przypisaną do wybranego wydziału (na podstawie obiektu transferowego).
        /// </summary>
        /// <param name="dto">obiekt transferowy z informacjami</param>
        /// <returns>informacje o utworzonej katedrze</returns>
        /// <exception cref="BasicServerException">nieistniejący wydział/duplikat katedry w systemie</exception>
        public async Task<CreatedCathedralResponseDto> CreateCathedral(CreateCathedralRequestDto dto)
        {
            //wyszukanie wydziału pasującego do katedry, jeśli nie znajdzie wyrzuci wyjątek
            Department findDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.Name.ToLower() == dto.DepartmentName.ToLower());
            if (findDepartment == null) {
                throw new BasicServerException("Nie znaleziono wydziału z podaną nazwą", HttpStatusCode.NotFound);
            }
            
            // przy próbie wprowadzeniu duplikatu katedry, wyrzuć wyjątek
            Cathedral findCathedral = await _context.Cathedrals
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => (c.Name.ToLower() == dto.Name.ToLower() 
                                           || c.Alias.ToLower() == dto.Alias.ToLower()) 
                                          && c.Department.Name.ToLower() == dto.DepartmentName.ToLower());
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

            return _mapper.Map<CreatedCathedralResponseDto>(cathedral);
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
        public async Task<SearchQueryResponseDto> GetAllCathedralsBasedDepartmentName(string cathName, string deptName)
        {
            if (deptName == null || deptName == string.Empty) {
                return new SearchQueryResponseDto()
                {
                    SearchQueryResults = new List<string>(),
                };
            }
            
            // wyszukaj wydziału na podstawie parametru, jeśli nie znajdzie zwróć pustą tablicę
            Department findDepartment = await _context.Departments
                .FirstOrDefaultAsync(d => d.Name.ToLower() == deptName.ToLower());
            if (findDepartment == null) {
                return new SearchQueryResponseDto()
                {
                    SearchQueryResults = new List<string>(),
                };
            }

            // jeśli parametr jest nullem to przypisz wartość pustego stringa
            if (cathName == null) {
                cathName = string.Empty;
            }
            
            // wyszukaj i wypłaszcz rezultat do tablicy stringów z nazwami katedr
            List<string> findAllCathedralsNames = _context.Cathedrals
                .Include(c => c.Department)
                .Where(c => c.Department.Name == findDepartment.Name && c.Name.ToLower().Contains(cathName.ToLower()))
                .Select(c => c.Name)
                .ToList();
            
            if (findAllCathedralsNames.Count > 0) {
                return new SearchQueryResponseDto()
                {
                    SearchQueryResults = findAllCathedralsNames,
                };
            }
            
            // jeśli nie znalazło pasujących rezultatów, zwróć wszystkie elementy
            return new SearchQueryResponseDto()
            {
                SearchQueryResults = _context.Cathedrals
                    .Include(c => c.Department)
                    .Where(c => c.Department.Name == findDepartment.Name)
                    .Select(c => c.Name)
                    .ToList(),
            };
        }

        #endregion
    }
}