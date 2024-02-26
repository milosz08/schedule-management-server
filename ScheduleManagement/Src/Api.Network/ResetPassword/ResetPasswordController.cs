using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Api.Dto;

namespace ScheduleManagement.Api.Network.ResetPassword;

[ApiController]
[Route("/api/v1/[controller]")]
public class ResetPasswordController(IResetPasswordService resetPasswordService) : ControllerBase
{
	[HttpPost("email")]
	public async Task<ActionResult<MessageContentResDto>> SendPasswordResetEmailToken([FromQuery] string userEmail)
	{
		return Ok(await resetPasswordService.SendPasswordResetEmailToken(userEmail));
	}

	[HttpPatch("email/check/token/{token}")]
	public async Task<ActionResult> ValidateResetPasswordToken([FromRoute] string token)
	{
		return Ok(await resetPasswordService.ValidateResetEmailToken(token));
	}

	[HttpPatch("email/change/token/{token}")]
	public async Task<ActionResult<SetNewPasswordViaEmailResponse>> ChangePasswordViaEmailToken(
		[FromRoute] string token,
		[FromBody] SetResetPasswordRequestDto dto)
	{
		return Ok(await resetPasswordService.ChangePasswordViaEmailToken(dto, token));
	}

	[HttpPatch("account/change")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public async Task<ActionResult<MessageContentResDto>> ChangePasswordViaAccount(
		[FromBody] ChangePasswordRequestDto form)
	{
		return Ok(await resetPasswordService.ChangePasswordViaAccount(form, HttpContext.User));
	}
}
