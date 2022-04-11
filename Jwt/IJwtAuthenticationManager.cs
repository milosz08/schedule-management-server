namespace asp_net_po_schedule_management_server.Jwt
{
    public interface IJwtAuthenticationManager
    {
        string BearerHandlingService(string username);
    }
}