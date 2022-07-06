/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: SearchContentServiceImplementation.cs
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

using System.Linq;
using System.Dynamic;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.DbConfig;
using asp_net_po_schedule_management_server.Entities;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class SearchContentServiceImplementation : ISearchContentService
    {
        private readonly ApplicationDbContext _context;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public SearchContentServiceImplementation(ApplicationDbContext context)
        {
            _context = context;
        }

        //--------------------------------------------------------------------------------------------------------------

        #region Search massive data on server

        /// <summary>
        /// Metoda obsługująca masywne przeszukiwania API w poszukiwaniu planów grup/pracowników/sal zajęciowych na
        /// podstawie parametrów zapytania zawartych w parametrach zapytania.
        /// </summary>
        /// <param name="query">parametry zapytania</param>
        /// <returns>tablica ze wszytkimi wyszukanymi elementami</returns>
        public async Task<List<SearchMassiveQueryResDto>> GetAllItemsFromMassiveServerQuery(SearchMassiveQueryReqDto query)
        {
            List<SearchMassiveQueryResDto> responseData = new List<SearchMassiveQueryResDto>();
            
            // wyszukiwanie wszystkich grup dziekańskich na podstawie nazwy, wydziału, kierunku studiów oraz aktywności
            // wyszukiwanego pola
            List<StudyGroup> findStudyGroups = await _context.StudyGroups
                .Include(g => g.Department)
                .Include(g => g.ScheduleSubjects)
                .Include(g => g.StudySpecialization)
                .Where(g => (g.Name.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                             g.StudySpecialization.StudyDegree.Name.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase) || 
                             g.StudySpecialization.StudyType.Name.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase) || 
                             g.StudySpecialization.Name.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase)) 
                            && query.IfGroupsActive)
                .ToListAsync();

            foreach (StudyGroup studyGroup in findStudyGroups) {
                dynamic queryParams = new ExpandoObject();
                queryParams.deptId = studyGroup.Department.Id;
                queryParams.specId = studyGroup.StudySpecialization.Id;
                queryParams.groupId = studyGroup.Id;
                    
                responseData.Add(new SearchMassiveQueryResDto()
                {
                    TypeName = "Grupa dziekańska",
                    DepartmentName = $"{studyGroup.Department.Name} ({studyGroup.Department.Alias})",
                    FullName = $"{studyGroup.Name}, {studyGroup.StudySpecialization.Name} ({studyGroup.StudySpecialization.Alias})",
                    PathParam = "groups",
                    PathQueryParams = queryParams,
                });
            }

            // wyszukiwanie wszystkich pracowników na podstawie imienia, nazwiska, wydziału, kierunku studiów, katedry
            // oraz aktywności wyszukiwanego pola
            List<Person> findStudyTeachers = await _context.Persons
                .Include(p => p.Role)
                .Include(p => p.Cathedral)
                .Include(p => p.Department)
                .Where(p => (p.Name.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                            p.Surname.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase))
                            && query.IfTeachersActive && p.Role.Name != AvailableRoles.STUDENT)
                .ToListAsync();
            
            foreach (Person person in findStudyTeachers) {
                dynamic queryParams = new ExpandoObject();
                queryParams.deptId = person.Department.Id;
                queryParams.cathId = person.Cathedral.Id;
                queryParams.employeerId = person.Id;
                    
                responseData.Add(new SearchMassiveQueryResDto()
                {
                    TypeName = $"Użytkownik, {person.Role.Name}",
                    DepartmentName = $"{person.Department.Name} ({person.Department.Alias})",
                    FullName = $"{person.Surname} {person.Name} ({person.Cathedral.Name})",
                    PathParam = "employeers",
                    PathQueryParams = queryParams,
                });
            }

            // wyszukiwanie wszystkich sal zajęciowych na podstawie nazwy, wydziału, katedry oraz aktywności
            // wyszukiwanego pola
            List<StudyRoom> findStudyRooms = await _context.StudyRooms
                .Include(p => p.RoomType)
                .Include(p => p.Cathedral)
                .Include(p => p.Department)
                .Where(p => (p.Name.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                             p.Description.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                             p.RoomType.Name.Contains(query.SearchQuery, StringComparison.OrdinalIgnoreCase))
                            && query.IfRoomsActive)
                .ToListAsync();
            
            foreach (StudyRoom studyRoom in findStudyRooms) {
                dynamic queryParams = new ExpandoObject();
                queryParams.deptId = studyRoom.Department.Id;
                queryParams.cathId = studyRoom.Cathedral.Id;
                queryParams.roomId = studyRoom.Id;
                    
                responseData.Add(new SearchMassiveQueryResDto()
                {
                    TypeName = $"Sala zajęciowa",
                    DepartmentName = $"{studyRoom.Department.Name} ({studyRoom.Department.Alias})",
                    FullName = $"{studyRoom.Name}, {studyRoom.Description} ({studyRoom.Cathedral.Name})",
                    PathParam = "rooms",
                    PathQueryParams = queryParams,
                });
            }
            
            return responseData;
        }

        #endregion
    }
}