using System.Threading.Tasks;


namespace asp_net_po_schedule_management_server.Ssh.StmpEmailService
{
    public interface IStmpEmailService
    {
        Task SendResetPassword(UserEmailOptions userEmailOptions);
    }
}