/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: GlobalConfigurer.cs
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


namespace asp_net_po_schedule_management_server.Utils
{
    /// <summary>
    /// Klasa mapująca wszystkie property z pliku konfiguracyjnego JSON.
    /// </summary>
    public class GlobalConfigurer
    {
        // obsługa JWT
        public static string JwtKey { get; set; }
        public static TimeSpan JwtExpiredTimestamp { get; set; }
        
        //--------------------------------------------------------------------------------------------------------------
        
        // obsługa SSH
        public static string SshServer { get; set; }
        public static string SshUsername { get; set; }
        public static string SshPassword { get; set; }
        public static string SshPasswordFieldName { get; set; }
        
        //--------------------------------------------------------------------------------------------------------------
        
        // obsługa email
        public static string SmtpSenderAddress { get; set; }
        public static string SmtpSenderDisplayName { get; set; }
        public static string SmtpUsername { get; set; }
        public static string SmtpPassword { get; set; }
        public static string SmtpHost { get; set; }
        public static int SmtpPort { get; set; }
        public static bool EnableSSL { get; set; }
        public static bool UseDefaultCredentials { get; set; }
        public static bool IsBodyHTML { get; set; }
        
        //--------------------------------------------------------------------------------------------------------------
        
        // inne
        public static string UserEmailDomain { get; set; }
        public static string DbDriverVersion { get; set; }
        public static byte UserEmailMaxSizeMb { get; set; }
        public static TimeSpan OptExpired { get; set; }
        public static InitialUserAccount InitialCredentials { get; set; }
        public static string ClientOrigin { get; set; }
        public static string DevClientOrigin { get; set; }
    }

    public class InitialUserAccount
    {
        public string AccountName { get; set; }
        public string AccountSurname { get; set; }
        public string AccountPassword { get; set; }
    }
}