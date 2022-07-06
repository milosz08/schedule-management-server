/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: SshEmailServiceImplementation.cs
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

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Ssh.SshInterceptor;


namespace asp_net_po_schedule_management_server.Ssh.SshEmailService
{
    /// <summary>
    /// Klasa odpowiedzialna za wykonywanie komend serwera do tworzenia/usuwania skrzynek email.
    /// </summary>
    public sealed class SshEmailServiceImplementation : ISshEmailService
    {
        private readonly ISshInterceptor _sshInterceptor;

        public SshEmailServiceImplementation(ISshInterceptor sshInterceptor)
        {
            _sshInterceptor = sshInterceptor;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Dodawanie nowej skrzynki pocztowej i ustawianie jej pojemności.
        /// </summary>
        /// <param name="emailAddress">adres email</param>
        /// <param name="emailPassword">hasło do adresu email</param>
        public void AddNewEmailAccount(string emailAddress, string emailPassword)
        {
            _sshInterceptor.ExecuteCommand(
                $"echo " +
                $"\"['--json', 'mail', 'account', 'add', '{emailAddress}', '{emailPassword}']\" " +
                $"| nc -U /var/run/devil2.sock"
            );
            // ustawienie maksymalnej pojemności skrzynki pocztowej
            _sshInterceptor.ExecuteCommand(
                $"devil mail quota {emailAddress} {GlobalConfigurer.UserEmailMaxSizeMb}M"
            );
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Aktualizacja hasła na podstawie adresu email
        /// </summary>
        /// <param name="emailAddress">adres email</param>
        /// <param name="newEmailPassword">nowe hasło do adresu email</param>
        public void UpdateEmailPassword(string emailAddress, string newEmailPassword)
        {
            _sshInterceptor.ExecuteCommand(
                $"echo " +
                $"\"['--json', 'mail', 'passwd', '{emailAddress}', '{newEmailPassword}']\" " +
                $"| nc -U /var/run/devil2.sock"
            );
        }
        
        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Usuwanie skrzynki pocztowej.
        /// </summary>
        /// <param name="emailAddress">adres email który ma zostać usunięty</param>
        public void DeleteEmailAccount(string emailAddress)
        {
            _sshInterceptor.ExecuteCommand(
                $"echo " +
                $"\"['--json', 'mail', 'account', 'del', '{emailAddress}']\" " +
                $"| nc -U /var/run/devil2.sock"
            );
        }
    }
}