/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: CathedralServiceImplementation.cs
 * Project name | Nazwa Projektu: asp-net-po-schedule-management-server
 *
 * Klient | Client: <https://github.com/Milosz08/Angular_PO_Schedule_Management_Client>
 * Serwer | Server: <https://github.com/Milosz08/ASP.NET_PO_Schedule_Management_Server>
 *
 * RestAPI for the Angular application to manage schedule for sample university. Written with the ASP.NET Core
 * and Entity Framework with mySQL database. Project for the teaching course "Objected Oriented Programming".
 *
 * RestAPI dla aplikacji Angular do zarządzania planem zajęć przykładowej uczelni wyższej. Napisane w oparciu o
 * ASP.NET Core oraz Entity Framework z bazą danych mySQL. Projekt wykonany na zajęcia "Programowanie Obiektowe".
 */

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

        #region Update cathedral

        /// <summary>
        /// Metoda odpowiedzialna za aktualizowanie danych wybranej katedry (na podstawie ciała zapytania i parametrów).
        /// </summary>
        /// <param name="dto">obiekt z danymi do zamiany</param>
        /// <param name="cathId">id katedry podlegającej zamianie</param>
        /// <returns>zamienione dane w postaci obiektu transferowego</returns>
        /// <exception cref="BasicServerException">jeśli nie znajdzie katedry/próba wprowadzenia tych samych danych</exception>
        public async Task<CathedralResponseDto> UpdateCathedral(CathedralRequestDto dto, long cathId)
        {
            // wyszukaj katedry na podstawie id, jeśli nie znajdzie, rzuć wyjątek
            Cathedral findCathedral = await _context.Cathedrals
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.Id == cathId);
            if (findCathedral == null) {
                throw new BasicServerException("Nie znaleziono katedry z podanym id", HttpStatusCode.NotFound);
            }
            // sprawdź, czy nie zachodzi próba dodania niezaktualizowanych wartości
            if (findCathedral.Name == dto.Name && findCathedral.Alias == dto.Alias) {
                throw new BasicServerException("Należy wprowadzić wartości różne od poprzednich.", 
                    HttpStatusCode.ExpectationFailed);
            }

            findCathedral.Name = dto.Name;
            findCathedral.Alias = dto.Alias;
            await _context.SaveChangesAsync();
            
            return _mapper.Map<CathedralResponseDto>(findCathedral);
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
        
        #region Get cathedral data base cathedral id

        /// <summary>
        /// Metoda pobierająca zawartość katedry z bazy danych na podstawie przekazywanego parametru id w parametrach
        /// zapytania HTTP. Metoda używana głównie w celu aktualizacji istniejących treści w serwisie.
        /// </summary>
        /// <param name="cathId">id katedry</param>
        /// <returns>obiekt transferowy z danymi konkretnej katedry</returns>
        /// <exception cref="BasicServerException">w przypadku nieznalezienia katedry z podanym id</exception>
        public async Task<CathedralEditResDto> GetCathedralBaseDbId(long cathId)
        {
            // wyszukaj katedrę na podstawie parametru ID w bazie danych, jeśli nie znajdzie rzuć 404.
            Cathedral findCathedral = await _context.Cathedrals
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.Id == cathId);
            if (findCathedral == null) {
                throw new BasicServerException("Nie znaleziono katedry z podanym numerem id.", HttpStatusCode.NotFound);
            }

            return _mapper.Map<CathedralEditResDto>(findCathedral);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Delete massive

        /// <summary>
        /// Metoda usuwająca wybrane katedry z bazy danych (na podstawie wartości id w ciele zapytania).
        /// </summary>
        /// <param name="cathedrals">wszystkie numery ID elementów do usunięcia</param>
        /// <param name="credentials">obiekt autoryzacji na podstawie claimów</param>
        public async Task DeleteMassiveCathedrals(MassiveDeleteRequestDto cathedrals, UserCredentialsHeaderDto credentials)
        {
            // sprawdź, czy usunięcie jest realizowane z konta administratora, jeśli nie wyrzuć wyjątek
            if (credentials.Person.Role.Name != AvailableRoles.ADMINISTRATOR) {
                throw new BasicServerException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora",
                    HttpStatusCode.Forbidden);
            }
            
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

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Delete all
        
        /// <summary>
        /// Metoda usuwająca z bazy danych wszystkie katedry (oprócz domyślnej zapisywanej przy seedowaniu).
        /// </summary>
        /// <param name="credentials">obiekt autoryzacji na podstawie claimów</param>
        public async Task DeleteAllCathedrals(UserCredentialsHeaderDto credentials)
        {
            // sprawdź, czy usunięcie jest realizowane z konta administratora, jeśli nie wyrzuć wyjątek
            if (credentials.Person.Role.Name != AvailableRoles.ADMINISTRATOR) {
                throw new BasicServerException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora",
                    HttpStatusCode.Forbidden);
            }
            
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