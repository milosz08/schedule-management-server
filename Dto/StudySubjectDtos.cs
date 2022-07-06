/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: StudySubjectDtos.cs
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

using System.ComponentModel.DataAnnotations;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class StudySubjectRequestDto
    {
        [Required(ErrorMessage = "Pole nazwy przedmiotu nie może być puste")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Pole nazwy wydziału nie może być puste")]
        public string DepartmentName { get; set; }
        
        [Required(ErrorMessage = "Pole nazwy kierunku nie może być puste")]
        public string StudySpecName { get; set; }
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class StudySubjectResponseDto
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string DepartmentFullName { get; set; }
        public string StudySpecFullName { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class StudySubjectQueryResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string SpecName { get; set; }
        public string SpecAlias { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentAlias { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class StudySubjectEditResDto
    {
        public string Name { get; set; }
        public string DepartmentName { get; set; }
        public string StudySpecName { get; set; }
    }
}