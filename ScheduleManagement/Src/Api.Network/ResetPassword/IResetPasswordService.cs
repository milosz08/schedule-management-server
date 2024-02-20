using System.Security.Claims;
using ScheduleManagement.Api.Dto;

namespace ScheduleManagement.Api.Network.ResetPassword;

public interface IResetPasswordService
{
	Task<MessageContentResDto> SendPasswordResetEmailToken(string userEmail);

	Task<SetNewPasswordViaEmailResponse> ValidateResetEmailToken(string token);

	Task<MessageContentResDto> ChangePasswordViaEmailToken(SetResetPasswordRequestDto dto, string token);

	Task<MessageContentResDto> ChangePasswordViaAccount(ChangePasswordRequestDto dto, ClaimsPrincipal claimsPrincipal);
}