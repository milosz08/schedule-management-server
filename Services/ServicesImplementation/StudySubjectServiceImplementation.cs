using System;
using AutoMapper;

using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Exceptions;
using asp_net_po_schedule_management_server.Services.Helpers;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class StudySubjectServiceImplementation : IStudySubjectService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly ServiceHelper _helper;

        //--------------------------------------------------------------------------------------------------------------
        
        public StudySubjectServiceImplementation(ApplicationDbContext context, IMapper mapper, ServiceHelper helper)
        {
            _context = context;
            _mapper = mapper;
            _helper = helper;
        }

        //--------------------------------------------------------------------------------------------------------------

        #region Add new study subject

        /// <summary>
        /// Metoda odpowiadająca za stworzenie nowego przedmiotu i dodanie go do bazy danych. Metoda sprawdza
        /// przed stworzeniem, czy nie zachodzi próba dodania duplikatu, jeśli tak rzuci wyjątek.
        /// </summary>
        /// <param name="dto">obiekt transferowy z danymi od klienta</param>
        /// <returns>informacje o stworzonym przedmiocie</returns>
        /// <exception cref="BasicServerException">brak zasobu/próba wprowadzenia duplikatu</exception>
        public async Task<CreateStudySubjectResponseDto> AddNewStudySubject(CreateStudySubjectRequestDto dto)
        {
            //wyszukanie kierunku oraz wydziału studiów pasującego do zapytania, jeśli nie znajdzie wyrzuci wyjątek
            StudySpecialization findSpecialization = await _context.StudySpecializations
                .Include(d => d.Department)
                .Include(d => d.StudyType)
                .Include(d => d.StudyDegree)
                .FirstOrDefaultAsync(s => string.Equals(s.Name + " (" + s.StudyType.Alias + " " +  s.StudyDegree.Alias + ")",
                                              dto.StudySpecName, StringComparison.OrdinalIgnoreCase) &&
                                          s.Department.Name.Equals(dto.DepartmentName, StringComparison.InvariantCulture));
            if (findSpecialization == null) {
                throw new BasicServerException(
                    "Nie znaleziono kierunku/wydziału z podaną nazwą", HttpStatusCode.NotFound);
            }
            
            // przy próbie wprowadzeniu duplikatu przedmiotu studiów, wyrzuć wyjątek
            StudySubject findSubject = await _context.StudySubjects
                .Include(s => s.Department)
                .Include(s => s.StudySpecialization)
                .FirstOrDefaultAsync(s => s.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase) &&
                                          s.StudySpecialization.Name.Equals(dto.StudySpecName, StringComparison.OrdinalIgnoreCase)
                                          && s.Department.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase));
            if (findSubject != null) {
                throw new BasicServerException(
                    "Podany przedmiot istnieje już na wybranym kierunku.", HttpStatusCode.ExpectationFailed);
            }

            StudySubject studySubject = new StudySubject()
            {
                Name = dto.Name,
                Alias = $"{ApplicationUtils.CreateSubjectAlias(dto.Name)}/{findSpecialization.Alias.ToUpper()}" +
                        $"/{findSpecialization.Department.Alias}",
                DepartmentId = findSpecialization.DepartmentId,
                StudySpecializationId = findSpecialization.Id,
            };
            
            await _context.AddAsync(studySubject);
            await _context.SaveChangesAsync();

            return _mapper.Map<CreateStudySubjectResponseDto>(studySubject);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Get all study subjects

        /// <summary>
        /// Metoda zwracająca wszystkie przedmioty opakowane w obiekt paginacji i fitrowania rezultatów na podstawie
        /// przekazywanych parametrów zapytania. Umożliwia sortowanie po kolumnach (kluczach) w trybach ASC/DES.
        /// </summary>
        /// <param name="searchQuery">parametry zapytania (filtrowania wyników)</param>
        /// <returns>opakowane dane wynikowe w obiekt paginacji</returns>
        public PaginationResponseDto<StudySubjectQueryResponseDto> GetAllStudySubjects(SearchQueryRequestDto searchQuery)
        {
            // wyszukiwanie użytkowników przy pomocy parametru SearchPhrase
            IQueryable<StudySubject> studySubjectsBaseQuery = _context.StudySubjects
                .Include(s => s.Department)
                .Include(s => s.StudySpecialization)
                .Include(s => s.StudySpecialization.StudyType)
                .Include(s => s.StudySpecialization.StudyDegree)
                .Where(s => searchQuery.SearchPhrase == null
                            || s.Name.Contains(searchQuery.SearchPhrase, StringComparison.OrdinalIgnoreCase));

            // sortowanie (rosnąco/malejąco) dla kolumn
            if (!string.IsNullOrEmpty(searchQuery.SortBy)) {
                _helper.PaginationSorting(new Dictionary<string, Expression<Func<StudySubject, object>>>
                {
                    { nameof(StudyRoom.Id), s => s.Id },
                    { nameof(StudyRoom.Name), s => s.Name },
                    { "DepartmentAlias", s => s.Department.Alias },
                    { "SpecTypeAlias", s => s.StudySpecialization.Alias },
                }, searchQuery, ref studySubjectsBaseQuery);
            }
            
            List<StudySubjectQueryResponseDto> allDepts = _mapper.Map<List<StudySubjectQueryResponseDto>>(_helper
                .PaginationAndAdditionalFiltering(studySubjectsBaseQuery, searchQuery));
            
            return new PaginationResponseDto<StudySubjectQueryResponseDto>(
                allDepts, studySubjectsBaseQuery.Count(), searchQuery.PageSize, searchQuery.PageNumber);
        }
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Get all study subjects base department

        /// <summary>
        /// Metoda zwracająca wszystkie wyszukane przedmioty na podstawie parametrów zapytania (nazwy przedmiotu i id
        /// przypisanego do niego wydziału oraz kierunku studiów). W przypadku braku znalezienia wyniku, zwraca
        /// wszystkie elementy z tabeli.
        /// </summary>
        /// <param name="subjcName">nazwa przedmiotu</param>
        /// <param name="deptId">id wydziału</param>
        /// <param name="studySpecId">id kierunku studiów</param>
        /// <returns>wszystkie elementy z tablicy/przefiltrowane wyniki</returns>
        public SearchQueryResponseDto GetAllStudySubjectsBaseDeptAndSpec(string subjcName, long deptId, long studySpecId)
        {
            // jeśli parametr jest nullem to przypisz wartość pustego stringa
            if (subjcName == null) {
                subjcName = string.Empty;
            }

            // wyszukaj i wypłaszcz rezultat do tablicy stringów z nazwami katedr
            List<string> findAllSubjects = _context.StudySubjects
                .Include(s => s.Department)
                .Include(s => s.StudySpecialization)
                .Where(s => s.Department.Id == deptId && s.StudySpecialization.Id == studySpecId &&
                            s.Name.Contains(subjcName, StringComparison.OrdinalIgnoreCase))
                .Select(s => s.Name)
                .ToList();
            findAllSubjects.Sort();
            
            if (findAllSubjects.Count > 0) {
                return new SearchQueryResponseDto(findAllSubjects);
            }

            List<string> findAllElements = _context.StudySubjects
                .Include(s => s.Department)
                .Where(s => s.Department.Id == deptId && s.StudySpecialization.Id == studySpecId)
                .Select(s => s.Name)
                .ToList();
            findAllElements.Sort();
            
            // jeśli nie znalazło pasujących rezultatów, zwróć wszystkie elementy
            return new SearchQueryResponseDto(findAllElements);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Get available subjects base department name

        /// <summary>
        /// Metoda pobierająca i zwracająca wszystkie dostępne przedmioty z bazy danych w postaci tupli (nazwa, id)
        /// na podstawie nazwy wydziału. Metoda nie posiada dynamicznego filtrowania wyników, zwraca jedynie statyczne
        /// dane pozyskane z bazy danych.
        /// </summary>
        /// <param name="deptName">nazwa wydziału</param>
        /// <returns>zwracane wyniki opakowane w obiekt transferowy</returns>
        public async Task<AvailableDataResponseDto<NameWithDbIdElement>> GetAvailableSubjectsBaseDept(string deptName)
        {
            List<StudySubject> findAllStudySubjects = await _context.StudySubjects
                .Include(s => s.Department)
                .Include(s => s.StudySpecialization)
                .Include(s => s.StudySpecialization.StudyType)
                .Include(s => s.StudySpecialization.StudyDegree)
                .Where(s => s.Department.Name.Equals(deptName, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();

            return new AvailableDataResponseDto<NameWithDbIdElement>(findAllStudySubjects
                .Select(s => _mapper.Map<NameWithDbIdElement>(s)).ToList());
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Delete content

        /// <summary>
        /// Metoda usuwająca wybrane przedmioty z bazy danych (na podstawie wartości id w ciele zapytania).
        /// </summary>
        /// <param name="studySpecs">wszystkie numery ID elementów do usunięcia</param>
        /// <param name="credentials">obiekt autoryzacji na podstawie claimów</param>
        public async Task DeleteMassiveSubjects(MassiveDeleteRequestDto subjects, UserCredentialsHeaderDto credentials)
        {
            await _helper.CheckIfUserCredentialsAreValid(credentials);
            // filtrowanie kierunków studiów po ID znajdujących się w tablicy
            _context.StudySubjects.RemoveRange(_context.StudySubjects
                .Where(s => subjects.ElementsIds.Any(id => id == s.Id)));
            await _context.SaveChangesAsync();
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Metoda usuwająca z bazy danych wszystkie przedmioty.
        /// </summary>
        /// <param name="credentials">obiekt autoryzacji na podstawie claimów</param>
        public async Task DeleteAllSubjects(UserCredentialsHeaderDto credentials)
        {
            await _helper.CheckIfUserCredentialsAreValid(credentials);
            _context.StudySubjects.RemoveRange();
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}