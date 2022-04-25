namespace asp_net_po_schedule_management_server.Ssh.SshEmailService
{
    public interface ISshEmailService
    {
        void AddNewEmailAccount(string emailAddress, string emailPassword);
        void UpdateEmailPassword(string emailAddress, string newEmailPassword);
        void DeleteEmailAccount(string emailAddress);
    }
}