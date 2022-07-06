/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: DepartmentDtos.cs
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
    public class DepartmentRequestResponseDto
    {
        [Required(ErrorMessage = "Pole nazwy wydziału nie może być puste")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Pole aliasu nazwy wydziału nie może być puste")]
        public string Alias { get; set; }
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class DepartmentQueryResponseDto : DepartmentRequestResponseDto
    {
        public long Id { get; set; }
        public bool IfRemovable { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class DepartmentEditResDto
    {
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}