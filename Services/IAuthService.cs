using System.Threading.Tasks;

using asp_net_po_schedule_management_server.Dto.AuthDtos;
using asp_net_po_schedule_management_server.Dto.Requests;
using asp_net_po_schedule_management_server.Dto.Responses;
using asp_net_po_schedule_management_server.Dto.CrossQuery;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> UserLogin(LoginRequestDto user);
        Task<RefreshTokenDto> UserRefreshToken(RefreshTokenDto dto);
        Task<RegisterNewUserResponseDto> UserRegister(RegisterNewUserRequestDto user);
        Task<PseudoNoContentResponseDto> UserChangePassword(ChangePasswordRequestDto dto, string userId);
    }
}