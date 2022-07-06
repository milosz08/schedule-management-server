/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: SshInterceptorImplementation.cs
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
using System.Net;
using System.Net.Sockets;

using Renci.SshNet;
using Renci.SshNet.Common;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Exceptions;


namespace asp_net_po_schedule_management_server.Ssh.SshInterceptor
{
    /// <summary>
    /// Klasa odpowiadająca za połączenie się poprzez SSH z serwerem w celu wykonywania na nim komend
    /// w konsoli (używana głównie do tworzenia/usuwania adresów email).
    /// </summary>
    public sealed class SshInterceptorImplementation : ISshInterceptor
    {
        private static readonly string PASS_FIELDNAME = GlobalConfigurer.SshPasswordFieldName;
        private static readonly string USR_NAME = GlobalConfigurer.SshUsername;
        private static readonly string SRV_HOST = GlobalConfigurer.SshServer;
        
        //--------------------------------------------------------------------------------------------------------------
        
        // klient subskrybujący połączenie SSH
        private SshClient _sshClient;
        
        public SshInterceptorImplementation()
        {
            Initialise();
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Initializacja połączenia z serwerem poprzez połączenie SSH.
        /// </summary>
        private void Initialise()
        {
            KeyboardInteractiveAuthenticationMethod keyAuth = new KeyboardInteractiveAuthenticationMethod(USR_NAME);
            keyAuth.AuthenticationPrompt += HandleKeyEvent;
            ConnectionInfo connectionInfo = new ConnectionInfo(SRV_HOST, 22, USR_NAME, keyAuth);
            _sshClient = new SshClient(connectionInfo);
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Wywołanie niestandardowej komendy (na podstawie parametru w commandParameter).
        /// </summary>
        /// <param name="commandParameter">komenda</param>
        /// <returns>rezultat wykonanej komendy</returns>
        /// <exception cref="BasicServerException">problem z nawiązaniem połączenia przez SSH/TELNET</exception>
        public string ExecuteCommand(string commandParameter)
        {
            string result;
            SshCommand customCommand;
            try {
                _sshClient.Connect();

                customCommand = _sshClient.CreateCommand(commandParameter);
                result = customCommand.Execute();
            }
            catch (SocketException ex) {
                throw new BasicServerException(
                    $"Nieudane połączenie z socketem SSH. Komunikat błędu: {ex}",
                    HttpStatusCode.GatewayTimeout);
            }
            catch (SshOperationTimeoutException ex) {
                throw new BasicServerException(
                    $"Zbyt długi czas wykonywania komendy. Potencjalny problem z połączeniem. Komunikat błędu: {ex}",
                    HttpStatusCode.GatewayTimeout);
            }
            finally {
                _sshClient.Disconnect();
            }
            return result;
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Listener autoryzujący dostep do powłoki.
        /// </summary>
        /// <param name="sender">nieużywane</param>
        /// <param name="eventArgs">argumenty wywołania</param>
        private static void HandleKeyEvent(object sender, AuthenticationPromptEventArgs eventArgs)
        {
            foreach (AuthenticationPrompt prompt in eventArgs.Prompts) {
                if (prompt.Request.IndexOf(PASS_FIELDNAME, StringComparison.InvariantCultureIgnoreCase) != -1) {
                    prompt.Response = GlobalConfigurer.SshPassword;
                }
            }
        }
    }
}