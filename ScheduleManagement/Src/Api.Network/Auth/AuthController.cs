using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Api.Attribute;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Network.User;

namespace ScheduleManagement.Api.Network.Auth;

[ApiController]
[Route("/api/v1/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class AuthController(IAuthService authService)
	: ControllerBase
{
	[AllowAnonymous]
	[HttpPost("login")]
	public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto user)
	{
		return Ok(await authService.Login(user));
	}

	[HttpPost("register")]
	[AuthorizeRoles(UserRole.Administrator)]
	public async Task<ActionResult<RegisterUpdateUserResponseDto>> Register(
		[FromBody] RegisterUpdateUserRequestDto user)
	{
		return Created(string.Empty, await authService.Register(user, string.Empty));
	}

	[AllowAnonymous]
	[HttpPatch("token/refresh")]
	public async Task<ActionResult<RefreshTokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto dto)
	{
		return Ok(await authService.RefreshToken(dto));
	}

	[HttpDelete("logout")]
	public async Task<ActionResult<MessageContentResDto>> Logout()
	{
		var refreshToken = Request.Headers["X-RefreshToken"][0] ?? "";
		return Ok(await authService.Logout(refreshToken, HttpContext.User));
	}
}