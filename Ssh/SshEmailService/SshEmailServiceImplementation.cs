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