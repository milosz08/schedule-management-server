/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: StudyGroupDtos.cs
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
    public sealed class CreateStudyGroupRequestDto
    {
        public string DepartmentName { get; set; }
        public string StudySpecName { get; set; }
        public List<long> Semesters { get; set; }
        public int CountOfGroups { get; set; }
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class CreateStudyGroupResponseDto
    {
        public string Name { get; set; }
        public string DepartmentFullName { get; set; }
        public string StudySpecFullName { get; set; }
        public string SemesterName { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class StudyGroupQueryResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentAlias { get; set; }
        public string StudySpecName { get; set; }
        public string StudySpecAlias { get; set; }
    }
}