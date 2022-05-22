using System;
using AutoMapper;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Entities;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class HelperServiceImplementation : IHelperService
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public HelperServiceImplementation(IMapper mapper, ApplicationDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        //--------------------------------------------------------------------------------------------------------------
        
        #region Get available pagination types

        /// <summary>
        /// Metoda pobierająca zamockowane dane służące do paginacji rezultatów na front-endzie.
        /// </summary>
        /// <returns>dostępne paginacje stron wyszukiwarki</returns>
        public AvailablePaginationSizes GetAvailablePaginationTypes()
        {
            return new AvailablePaginationSizes(ApplicationUtils._allowedPageSizes.ToList());
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Get available study types

        /// <summary>
        /// Metoda pobierająca i zwracająca wszystkie typy studiów (stacjonarne, zaoczne, itp.) z bazy danych w postaci
        /// tupli (nazwa, id).
        /// </summary>
        /// <returns>zwracane wyniki opakowane w obiekt transferowy</returns>
        public async Task<AvailableDataResponseDto<NameWithDbIdElement>> GetAvailableStudyTypes()
        {
            // wypłaszczanie wyniku i mapowanie na obiekt transferowy (DTO)
            return new AvailableDataResponseDto<NameWithDbIdElement>(await _context.StudyTypes
                .Select(t => _mapper.Map<NameWithDbIdElement>(t)).ToListAsync());
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Get available study degrees

        /// <summary>
        /// Metoda pobierająca i zwracająca wszystkie stopnie studiów z bazy danych w postaci tupli (nazwa, id).
        /// </summary>
        /// <returns>zwracane wyniki opakowane w obiekt transferowy</returns>
        public async Task<AvailableDataResponseDto<NameWithDbIdElement>> GetAvailableStudyDegreeTypes()
        {
            // wypłaszczanie wyniku i mapowanie na obiekt transferowy (DTO)
            return new AvailableDataResponseDto<NameWithDbIdElement>(await _context.StudyDegrees
                .Select(d => _mapper.Map<NameWithDbIdElement>(d)).ToListAsync());
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Get available semesters

        /// <summary>
        /// Metoda pobierająca i zwracająca wszystkie semestry z bazy danych w postaci tupli (nazwa, id).
        /// </summary>
        /// <returns>zwracane wyniki opakowane w obiekt transferowy</returns>
        public async Task<AvailableDataResponseDto<NameWithDbIdElement>> GetAvailableSemesters()
        {
            // wypłaszczanie wyniku i mapowanie na obiekt transferowy (DTO)
            return new AvailableDataResponseDto<NameWithDbIdElement>(await _context.Semesters
                .Select(s => _mapper.Map<NameWithDbIdElement>(s)).ToListAsync());
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------
        
        #region Get available study degrees base study specializations in selected department

        /// <summary>
        /// Metoda zwracająca przefiltrowaną listę pozimów studiów (I lub II stopnia) na podstawie id wydziału
        /// przekazywanego w parametrach zapytania.
        /// </summary>
        /// <param name="deptId">id wydziału w bazie danych</param>
        /// <returns>lista przefiltrowanych poziomów studiów (usuwane duplikaty)</returns>
        public async Task<List<NameWithDbIdElement>> GetAvailableStudyDegreeBaseAllSpecs(long deptId)
        {
            List<StudyDegree> findAllDegreesBaseAllSpecs = await _context.StudySpecializations
                .Include(s => s.StudyDegree)
                .Include(s => s.Department)
                .Where(s => s.Department.Id == deptId)
                .Select(r => r.StudyDegree)
                .ToListAsync();
            
            // usuwanie duplikatów i sortowanie
            List<StudyDegree> removeDuplicates = findAllDegreesBaseAllSpecs.Distinct().ToList();
            removeDuplicates
                .Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.OrdinalIgnoreCase));
            
            // wypłaszczanie i mapowanie na obiekt transferowy
            return removeDuplicates.Select(d => _mapper.Map<NameWithDbIdElement>(d)).ToList();
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------

        #region Get available semesters base study groups in selected study specializations
        
        /// <summary>
        /// Metoda zwracająca przefiltrowaną listę semestrów na podstawie id wydziału i id kierunku studiów
        /// przekazywanych w parametrach zapytania.
        /// </summary>
        /// <param name="deptId">id wydziału w bazie danych</param>
        /// <param name="studySpecId">id kierunku studiów w bazie danych</param>
        /// <returns>lista przefiltrowanych semestrów (usuwane duplikaty)</returns>
        public async Task<List<NameWithDbIdElement>> GetAvailableSemBaseStudyGroups(
            long deptId, long studySpecId)
        {
            List<Semester> findAllSemestersBaseSpec = await _context.StudyGroups
                .Include(g => g.StudySpecialization)
                .Include(g => g.Department)
                .Include(g => g.Semester)
                .Where(g => g.StudySpecialization.Id == studySpecId && g.Department.Id == deptId)
                .Select(g => g.Semester)
                .ToListAsync();
            
            // usuwanie duplikatów i sortowanie
            List<Semester> removeDuplicates = findAllSemestersBaseSpec.Distinct().ToList();
            removeDuplicates
                .Sort((first, second) => string.Compare(first.Name, second.Name, StringComparison.OrdinalIgnoreCase));
            
            // wypłaszczanie i mapowanie na obiekt transferowy
            return removeDuplicates.Select(d => _mapper.Map<NameWithDbIdElement>(d)).ToList();
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
        
        #region Get available study room types
        
        /// <summary>
        /// Metoda pobierająca i zwracająca wszystkie typy sal zajęciowych z bazy danych w postaci tablicy stringów.
        /// </summary>
        /// <returns>zwracane wyniki opakowane w obiekt transferowy</returns>
        public async Task<AvailableDataResponseDto<string>> GetAvailableRoomTypes()
        {
            // wypłaszczanie wyniku i mapowanie na obiekt transferowy (DTO)
            return new AvailableDataResponseDto<string>(await _context.RoomTypes
                .Select(r => $"{r.Name} ({r.Alias})").ToListAsync());
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Get available user roles

        /// <summary>
        /// Metoda pobierająca i zwracająca wszystkie role z bazy danych w postaci tablicy stringów.
        /// </summary>
        /// <returns>zwracane wyniki opakowane w obiekt transferowy</returns>
        public async Task<AvailableDataResponseDto<string>> GetAvailableRoles()
        {
            // wypłaszczanie wyniku i mapowanie na obiekt transferowy (DTO)
            return new AvailableDataResponseDto<string>(await _context.Roles.Select(r => r.Name).ToListAsync());
        }

        #endregion
    }
}