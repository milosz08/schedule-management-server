using System.Threading.Tasks;


namespace asp_net_po_schedule_management_server.Ssh.SmtpEmailService
{
    public interface ISmtpEmailService
    {
        Task SendResetPassword(UserEmailOptions userEmailOptions);
    }
}