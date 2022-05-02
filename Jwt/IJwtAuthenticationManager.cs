using System.Security.Claims;
using asp_net_po_schedule_management_server.Entities;


namespace asp_net_po_schedule_management_server.Jwt
{
    public interface IJwtAuthenticationManager
    {
        string BearerHandlingService(Person person);
        string BearerHandlingResetPasswordTokenService(Person person, string otpToken);
        string RefreshTokenGenerator();
        string BearerHandlingRefreshTokenService(Claim[] claims);
    }
}