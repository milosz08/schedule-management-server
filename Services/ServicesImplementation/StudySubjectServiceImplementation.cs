﻿/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: StudySubjectServiceImplementation.cs
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
        public async Task<StudySubjectResponseDto> AddNewStudySubject(StudySubjectRequestDto dto)
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
                Alias = $"{ApplicationUtils.CreateSubjectAlias(dto.Name)}/{findSpecialization.Alias}" +
                        $"/{findSpecialization.Department.Alias}",
                DepartmentId = findSpecialization.DepartmentId,
                StudySpecializationId = findSpecialization.Id,
            };
            
            await _context.AddAsync(studySubject);
            await _context.SaveChangesAsync();

            return _mapper.Map<StudySubjectResponseDto>(studySubject);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Update study subject

        /// <summary>
        /// Metoda odpowiedzialna za aktualizowanie danych wybranego przedmiotu (na podstawie ciała zapytania i parametrów).
        /// </summary>
        /// <param name="dto">obiekt z danymi do zamiany</param>
        /// <param name="subjId">id przedmiotu podlegającego zamianie</param>
        /// <returns>zamienione dane w postaci obiektu transferowego</returns>
        /// <exception cref="BasicServerException">jeśli nie znajdzie przedmiotu/próba wprowadzenia tych samych danych</exception>
        public async Task<StudySubjectResponseDto> UpdateStudySubject(StudySubjectRequestDto dto, long subjId)
        {
            // wyszukaj przedmiot w bazie danych, jeśli nie znajdzie rzuć wyjątek
            StudySubject findStudySubject = await _context.StudySubjects
                .Include(s => s.Department)
                .Include(s => s.StudySpecialization).ThenInclude(sp => sp.Department)
                .FirstOrDefaultAsync(s => s.Id == subjId);
            if (findStudySubject == null) {
                throw new BasicServerException("Nie znaleziono przedmiotu z podanym id", HttpStatusCode.NotFound);
            }
            
            // sprawdź, czy nie zachodzi próba dodania niezaktualizowanych wartości, jeśli tak rzuć wyjątek
            if (findStudySubject.Name == dto.Name) {
                throw new BasicServerException("Należy wprowadzić wartości różne od poprzednich.", 
                    HttpStatusCode.ExpectationFailed);
            }

            findStudySubject.Name = dto.Name;
            findStudySubject.Alias = $"{ApplicationUtils.CreateSubjectAlias(dto.Name)}" +
                                     $"/{findStudySubject.StudySpecialization.Alias}" +
                                     $"/{findStudySubject.StudySpecialization.Department.Alias}";
            
            await _context.SaveChangesAsync();
            return _mapper.Map<StudySubjectResponseDto>(findStudySubject);
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
        
        #region Get study subject data base study subject id

        /// <summary>
        /// Metoda pobierająca zawartość przedmiotu z bazy danych na podstawie przekazywanego parametru id w
        /// parametrach zapytania HTTP. Metoda używana głównie w celu aktualizacji istniejących treści w serwisie.
        /// </summary>
        /// <param name="specId">id przedmiotu</param>
        /// <returns>obiekt transferowy z danymi konkretnego przedmiotu</returns>
        /// <exception cref="BasicServerException">w przypadku nieznalezienia przedmiotu z podanym id</exception>
        public async Task<StudySubjectEditResDto> GetStudySubjectBaseDbId(long subjId)
        {
            // wyszukaj katedrę na podstawie parametru ID w bazie danych, jeśli nie znajdzie rzuć 404.
            StudySubject findStudySubject = await _context.StudySubjects
                .Include(s => s.Department)
                .Include(s => s.StudySpecialization).ThenInclude(sp => sp.StudyType)
                .Include(s => s.StudySpecialization).ThenInclude(sp => sp.StudyDegree)
                .FirstOrDefaultAsync(s => s.Id == subjId);
            if (findStudySubject == null) {
                throw new BasicServerException("Nie znaleziono przedmiotu z podanym numerem id.", HttpStatusCode.NotFound);
            }

            return _mapper.Map<StudySubjectEditResDto>(findStudySubject);
        }

        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        #region Delete massive

        /// <summary>
        /// Metoda usuwająca wybrane przedmioty z bazy danych (na podstawie wartości id w ciele zapytania).
        /// </summary>
        /// <param name="studySpecs">wszystkie numery ID elementów do usunięcia</param>
        /// <param name="credentials">obiekt autoryzacji na podstawie claimów</param>
        public async Task DeleteMassiveSubjects(MassiveDeleteRequestDto subjects, UserCredentialsHeaderDto credentials)
        {
            // sprawdź, czy usunięcie jest realizowane z konta administratora, jeśli nie wyrzuć wyjątek
            if (credentials.Person.Role.Name != AvailableRoles.ADMINISTRATOR) {
                throw new BasicServerException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora.",
                    HttpStatusCode.Forbidden);
            }
            
            // filtrowanie kierunków studiów po ID znajdujących się w tablicy
            _context.StudySubjects.RemoveRange(_context.StudySubjects
                .Where(s => subjects.ElementsIds.Any(id => id == s.Id)));
            await _context.SaveChangesAsync();
        }
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------

        #region Delete all

        /// <summary>
        /// Metoda usuwająca z bazy danych wszystkie przedmioty.
        /// </summary>
        /// <param name="credentials">obiekt autoryzacji na podstawie claimów</param>
        public async Task DeleteAllSubjects(UserCredentialsHeaderDto credentials)
        {
            // sprawdź, czy usunięcie jest realizowane z konta administratora, jeśli nie wyrzuć wyjątek
            if (credentials.Person.Role.Name != AvailableRoles.ADMINISTRATOR) {
                throw new BasicServerException("Nastąpiła próba usunięcia zasobu z konta bez rangi administratora.",
                    HttpStatusCode.Forbidden);
            }
            
            _context.StudySubjects.RemoveRange();
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}