/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: ContactMessagesDtos.cs
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
    public sealed class ContactMessagesReqDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string UserHash { get; set; }
        public string DepartmentName { get; set; }
        public List<long> Groups { get; set; }
        
        [Required(ErrorMessage = "Pole typu zgłoszenia nie może być puste")]
        public string IssueType { get; set; }
        
        [Required(ErrorMessage = "Pole opisu zgłoszenia nie może być puste")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Pole statusu zalogowania przy zgłoszeniu nie może być puste")]
        public bool IfAnonymous { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class ContactMessagesQueryResponseDto
    {
        public long Id { get; set; }
        public string NameWithSurname { get; set; }
        public string IssueType { get; set; }
        public bool IfAnonymous { get; set; }
        public string CreatedDate { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class SingleContactMessageResponseDto
    {
        public string NameWithSurname { get; set; }
        public string Email { get; set; }
        public string MessageIdentifier { get; set; }
        public string IssueType { get; set; }
        public string DepartmentName { get; set; } = "brak";
        public List<string> Groups { get; set; } = new List<string>();
        public bool IfAnonymous { get; set; }
        public string Description { get; set; }
        public string CreatedDate { get; set; }
    }
}