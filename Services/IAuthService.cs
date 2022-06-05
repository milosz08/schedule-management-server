using System.Threading.Tasks;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> UserLogin(LoginRequestDto user);
        Task<RefreshTokenResponseDto> UserRefreshToken(RefreshTokenRequestDto dto);
        Task<RegisterUpdateUserResponseDto> UserRegister(RegisterUpdateUserRequestDto user, string customPassword);
    }
}