/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: LoginDtos.cs
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
using System.ComponentModel.DataAnnotations;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class LoginRequestDto
    {
        [Required(ErrorMessage = "Pole loginu nie może być puste")]
        public string Login { get; set; }
        
        [Required(ErrorMessage = "Pole hasła nie może być puste")]
        public string Password { get; set; }
    }
    
    public sealed class LoginResponseDto
    {
        public string DictionaryHash { get; set; }
        public string BearerToken { get; set; }
        public string RefreshBearerToken { get; set; }
        public string Role { get; set; }
        public string NameWithSurname { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public DateTime TokenExpirationDate { get; set; }
        public double TokenRefreshInSeconds { get; set; }
        public bool FirstAccess { get; set; }
        public bool HasPicture { get; set; }
        public string ConnectedWithDepartment { get; set; }
    }
}