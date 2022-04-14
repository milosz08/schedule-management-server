using asp_net_po_schedule_management_server.Dto.AuthDtos;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IAuthService
    {
        LoginResponseDto UserLogin(LoginRequestDto user);
        RegisterNewUserResponseDto UserRegister(RegisterNewUserRequestDto user);
    }
}