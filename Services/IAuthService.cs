using System.Security.Claims;
using System.Threading.Tasks;

using asp_net_po_schedule_management_server.Dto.Requests;
using asp_net_po_schedule_management_server.Dto.Responses;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> UserLogin(LoginRequestDto user);
        Task<RefreshTokenResponseDto> UserRefreshToken(RefreshTokenRequestDto dto);
        Task<RegisterNewUserResponseDto> UserRegister(RegisterNewUserRequestDto user);
        Task<PseudoNoContentResponseDto> UserChangePassword(ChangePasswordRequestDto dto, string userId, Claim userLogin);
    }
}