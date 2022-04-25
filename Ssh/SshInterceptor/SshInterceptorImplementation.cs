using System;
using System.Net;
using System.Net.Sockets;

using Renci.SshNet;
using Renci.SshNet.Common;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Exceptions;


namespace asp_net_po_schedule_management_server.Ssh.SshInterceptor
{
    // klasa odpowiadająca za połączenie się poprzez SSH z serwerem w celu wykonywania na nim komend
    // w konsoli (używana głównie do tworzenia/usuwania adresów email).
    public sealed class SshInterceptorImplementation : ISshInterceptor
    {
        private static readonly string PASS_FIELDNAME = GlobalConfigurer.SshPasswordFieldName;
        private static readonly string USR_NAME = GlobalConfigurer.SshUsername;
        private static readonly string SRV_HOST = GlobalConfigurer.SshServer;
        
        // klient subskrybujący połączenie SSH
        private SshClient _sshClient;
        
        public SshInterceptorImplementation()
        {
            Initialise();
        }

        // initializacja połączenia z serwerem poprzez połączenie SSH
        private void Initialise()
        {
            KeyboardInteractiveAuthenticationMethod keyAuth = new KeyboardInteractiveAuthenticationMethod(USR_NAME);
            keyAuth.AuthenticationPrompt += HandleKeyEvent;
            ConnectionInfo connectionInfo = new ConnectionInfo(SRV_HOST, 22, USR_NAME, keyAuth);
            _sshClient = new SshClient(connectionInfo);
        }

        // wywołanie customowej komendy (na podstawie parametru w commandParameter)
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

        // listener autoryzujący dostep do powłoki
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