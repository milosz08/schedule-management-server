using System.Security.Claims;
using System.Threading.Tasks;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IResetPasswordService
    {
        Task<PseudoNoContentResponseDto> SendPasswordResetTokenViaEmail(string userEmail);
        Task<SetNewPasswordViaEmailResponse> ResetPasswordViaEmailToken(string emailToken);
        Task<PseudoNoContentResponseDto> UserResetPassword(SetResetPasswordRequestDto dto, Claim resetToken, Claim userLogin);
        Task<PseudoNoContentResponseDto> UserChangePassword(ChangePasswordRequestDto dto, string userId, Claim userLogin);
    }
}