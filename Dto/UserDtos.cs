/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: UserDtos.cs
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

using System.Collections.Generic;

namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class UserResponseDto
    {
        public long Id { get; set; }
        public string NameWithSurname { get; set; }
        public string Login { get; set; }
        public string Role { get; set; }
        public bool IfRemovable { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class DashboardDetailsResDto
    {
        public string Email { get; set; }
        public string Shortcut { get; set; }
        public string City { get; set; }
        public string Nationality { get; set; }
        public string DepartmentFullName { get; set; }
        public string CathedralFullName { get; set; }
        public List<string> StudySpecializations { get; set; } = new List<string>();
        public List<string> StudySubjects { get; set; } = new List<string>();
        public DashboardElementsCount DashboardElementsCount;
        public DashboardUserTypesCount DashboardUserTypesCount;
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class DashboardElementsCount
    {
        public int DepartmentsCount { get; set; }
        public int CathedralsCount { get; set; }
        public int StudyRoomsCount { get; set; }
        public int StudySpecializationsCount { get; set; }
        public int StudySubjectsCount { get; set; }
        public int StudyGroupsCount { get; set; }
        public int AllElements { get; set; }
        
        public DashboardElementsCount(int dept, int cath, int room, int spec, int subj, int group)
        {
            DepartmentsCount = dept;
            CathedralsCount = cath;
            StudyRoomsCount = room;
            StudySpecializationsCount = spec;
            StudySubjectsCount = subj;
            StudyGroupsCount = group;
            AllElements = dept + cath + room + spec + subj + group;
        }
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class DashboardUserTypesCount
    {
        public int StudentsCount { get; set; }
        public int TeachersCount { get; set; }
        public int EditorsCount { get; set; }
        public int AdministratorsCount { get; set; }
        public int AllElements { get; set; }

        public DashboardUserTypesCount(int stud, int teach, int edit, int admin)
        {
            StudentsCount = stud;
            TeachersCount = teach;
            EditorsCount = edit;
            AdministratorsCount = admin;
            AllElements = stud + teach + edit + admin;
        }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class UserDetailsEditResDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string City { get; set; }
        public string Nationality { get; set; }
        public string Role { get; set; }
        public string DepartmentName { get; set; }
        public string CathedralName { get; set; }
        public List<long> StudySpecsOrSubjects { get; set; }
    }
}