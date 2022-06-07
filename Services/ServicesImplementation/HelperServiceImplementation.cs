using System;
using AutoMapper;

using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Exceptions;


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

        //--------------------------------------------------------------------------------------------------------------

        #region Get available schedule subject types

        /// <summary>
        /// Metoda pobierająca i zwracająca wszystkie typy przedmiotów z bazy danych w postaci tablicy stringów na
        /// podstawie parametru zapytania.
        /// </summary>
        /// <param name="typeName">nazwa typu</param>
        /// <returns>zwracane wyniki opakowane w obiekt transferowy</returns>
        public async Task<AvailableDataResponseDto<string>> GetAvailableSubjectTypes(string subjTypeName)
        {
            if (subjTypeName == null) {
                subjTypeName = string.Empty;
            }
            
            List<string> findAllScheduleSubjectTypes = await _context.ScheduleSubjectTypes
                .Where(t => t.Name.Contains(subjTypeName, StringComparison.OrdinalIgnoreCase) || subjTypeName == string.Empty)
                .Select(t => t.Name)
                .ToListAsync();
            
            return new AvailableDataResponseDto<string>(findAllScheduleSubjectTypes);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Convert names to ids

        /// <summary>
        /// Metoda odpowiedzialna za konwertowanie parametrów zapytania grupy z parametrów typu string na parametry
        /// numeryczne (wartości id) w bazie danych.
        /// </summary>
        /// <param name="dto">parametry typu string</param>
        /// <returns>parametry numeryczne</returns>
        /// <exception cref="BasicServerException">w przypadku braku obiektu na podstawie parametrów</exception>
        public async Task<ConvertToNameWithIdResponseDto> ConvertNamesToIds(ConvertNamesToIdsRequestDto dto)
        {
            // wyszukiwanie grupy dziekańskiej na podstawie podanych parametrów
            StudyGroup findStudyGroup = await _context.StudyGroups
                .Include(g => g.Department)
                .Include(g => g.StudySpecialization)
                .FirstOrDefaultAsync(g => g.Name.Equals(dto.StudyGroupName, StringComparison.OrdinalIgnoreCase) &&
                                          g.Department.Name.Equals(dto.DepartmentName, StringComparison.OrdinalIgnoreCase) &&
                                          string.Equals(g.StudySpecialization.Name + " (" +
                                                        g.StudySpecialization.StudyType.Alias + " " +
                                                        g.StudySpecialization.StudyDegree.Alias + ")",
                                              dto.StudySpecName, StringComparison.OrdinalIgnoreCase));
            // jeśli nie znajdzie żadnej pasującej grupy, rzuć wyjątek
            if (findStudyGroup == null) {
                throw new BasicServerException("Nie znaleziono grupy z podanymi parametrami.", HttpStatusCode.NotFound);
            }

            // zwrócenie przekonwertowanych danych na obiekt transferowy DTO
            return new ConvertToNameWithIdResponseDto(
                findStudyGroup.Department,
                findStudyGroup.StudySpecialization,
                findStudyGroup);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Convert ids to names

        /// <summary>
        /// Metoda odpowiedzialna za konwertowanie parametrów zapytania grupy z parametrów numerycznych na parametry
        /// typu string (nazwa z bazy danych).
        /// </summary>
        /// <param name="dto">parametry numeryczne</param>
        /// <returns>parametry typu string</returns>
        /// <exception cref="BasicServerException">w przypadku braku obiektu na podstawie parametrów</exception>
        public async Task<ConvertToNameWithIdResponseDto> ConvertIdsToNames(ConvertIdsToNamesRequestDto dto)
        {
            if (dto.StudySpecId == null || dto.StudyGroupId == null || dto.DepartmentId == null) {
                throw new BasicServerException("Niepoprawne parametry planu.", HttpStatusCode.NotFound);
            }
            
            // wyszukiwanie grupy dziekańskiej na podstawie podanych parametrów
            StudyGroup findStudyGroup = await _context.StudyGroups
                .Include(g => g.Department)
                .Include(g => g.StudySpecialization)
                .FirstOrDefaultAsync(g => g.Id == dto.StudyGroupId && g.Department.Id == dto.DepartmentId &&
                                          g.StudySpecialization.Id == dto.StudySpecId);
            // jeśli nie znajdzie żadnej pasującej grupy, rzuć wyjątek
            if (findStudyGroup == null) {
                throw new BasicServerException("Nie znaleziono grupy z podanymi parametrami.", HttpStatusCode.NotFound);
            }

            // zwrócenie przekonwertowanych danych na obiekt transferowy DTO
            return new ConvertToNameWithIdResponseDto(
                findStudyGroup.Department,
                findStudyGroup.StudySpecialization,
                findStudyGroup);
        }

        #endregion
    }
}