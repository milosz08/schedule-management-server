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