/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: StudySpecDtos.cs
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
using System.ComponentModel.DataAnnotations;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class StudySpecRequestDto
    {
        [Required(ErrorMessage = "Pole nazwy kierunku nie może być puste")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Pole aliasu nazwy kierunku nie może być puste")]
        public string Alias { get; set; }
        
        [Required(ErrorMessage = "Pole nazwy przypisanego wydziału do kierunku nie może być puste")]
        public string DepartmentName { get; set; }
        
        [Required(ErrorMessage = "Pole typu/typów kierunku do kierunku nie może być puste")]
        public List<long> StudyType { get; set; }
        
        [Required(ErrorMessage = "Pole stopnia/stopni studiów nie może być puste")]
        public List<long> StudyDegree { get; set; }
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class StudySpecResponseDto
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string DepartmentFullName { get; set; }
        public string StudyTypeFullName { get; set; }
        public string StudyDegreeFullName { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class StudySpecQueryResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string SpecTypeName { get; set; }
        public string SpecTypeAlias { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentAlias { get; set; }
        public string StudyDegree { get; set; }
        public string StudyDegreeAlias { get; set; }
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class StudySpecializationEditResDto
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string DepartmentName { get; set; }
        public List<long> StudyType { get; set; }
        public List<long> StudyDegree { get; set; }
    }
}