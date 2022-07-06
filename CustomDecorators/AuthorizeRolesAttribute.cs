/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: AuthorizeRolesAttribute.cs
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

using Microsoft.AspNetCore.Authorization;


namespace asp_net_po_schedule_management_server.CustomDecorators
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Metoda zmieniająca działanie domyślnego dekoratora Authorize, umożliwiając przesłanie
        /// wartości innych niż stringi (w tym wypadku enumy) w jednym ciągu.
        /// </summary>
        /// <param name="allowedRoles">tablica parametrów dozwolonych ról</param>
        public AuthorizeRolesAttribute(params string[] allowedRoles)
        {
            Roles = string.Join(",", allowedRoles);
        }
    }
}