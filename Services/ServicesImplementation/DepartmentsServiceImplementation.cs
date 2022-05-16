using AutoMapper;

using System.Net;
using System.Linq;
using System.Collections.Generic;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Dto.Responses;
using asp_net_po_schedule_management_server.Dto.RequestResponseMerged;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class DepartmentsServiceImplementation : IDepartmentsService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        //--------------------------------------------------------------------------------------------------------------

        public DepartmentsServiceImplementation(IMapper mapper, ApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
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
        public async Task<CreateDepartmentRequestResponseDto> CreateDepartment(CreateDepartmentRequestResponseDto dto)
        {
            // przy próbie wprowadzeniu duplikatu wydziału, wyrzuć wyjątek
            Department findDepartment = await _context.Departments.FirstOrDefaultAsync(d =>
                d.Name.ToLower() == dto.Name.ToLower() || d.Alias.ToLower() == dto.Alias.ToLower());
            if (findDepartment != null) {
                throw new BasicServerException(
                    "Podany wydział istnieje już w systemie.", HttpStatusCode.ExpectationFailed);
            }

            // mapowanie obiektu DTO na instancję encji dodawaną do bazy danych
            Department newDepartment = _mapper.Map<Department>(dto);
            newDepartment.Name = newDepartment.Name.ToLower();
            await _context.Departments.AddAsync(newDepartment);
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
        public SearchQueryResponseDto GetAllDepartmentsList(string deptQuerySearch)
        {
            if (deptQuerySearch == null || deptQuerySearch == string.Empty) {
                List<string> allDepartments = _context.Departments.Select(d => d.Name).ToList();
                allDepartments.Sort();
                return new SearchQueryResponseDto()
                {
                    SearchQueryResults = allDepartments,
                };
            }

            // spłaszczanie i sortowanie wyniku pobrania wszystkich wydziałów na podstawie parametru wyszukiwania
            List<string> findAllDepartments = _context.Departments
                .Where(d => d.Name.ToLower().Contains(deptQuerySearch.ToLower()))
                .Select(d => d.Name)
                .ToList();

            if (findAllDepartments.Count > 0) {
                return new SearchQueryResponseDto()
                {
                    SearchQueryResults = findAllDepartments,
                };
            }
            
            return new SearchQueryResponseDto()
            {
                SearchQueryResults = new List<string>(),
            };
        }

        #endregion
    }
}