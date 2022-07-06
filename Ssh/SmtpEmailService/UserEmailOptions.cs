/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: UserEmailOptions.cs
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


namespace asp_net_po_schedule_management_server.Ssh.SmtpEmailService
{
    /// <summary>
    /// Klasa przechowująca parametry wysyłanej wiadomości email.
    /// </summary>
    public class UserEmailOptions
    {
        // Lista adresów email do jakich system wyśle emaile.
        public List<string> ToEmails { get; set; }
        
        // Tytuł emailu.
        public string Subject { get; set; }
        
        // Ciało emailu (szablon HTML).
        public string Body { get; set; }
        
        // Literały szablonowe zastępowane przez zmienne.
        public List<KeyValuePair<string, string>> Placeholders { get; set; }
    }
}