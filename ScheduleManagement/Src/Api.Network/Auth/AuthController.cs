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
	public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto reqDto)
	{
		return Ok(await authService.Login(reqDto));
	}

	[AllowAnonymous]
	[HttpPost("login/token")]
	public async Task<ActionResult<LoginResponseDto>> LoginToken([FromBody] TokenLoginRequestDto reqDto)
	{
		return Ok(await authService.TokenLogin(reqDto));
	}

	[HttpPost("register")]
	[AuthorizeRoles(UserRole.Administrator)]
	public async Task<ActionResult<RegisterUpdateUserResponseDto>> Register(
		[FromBody] RegisterUpdateUserRequestDto reqDto)
	{
		return Created(string.Empty, await authService.Register(reqDto, string.Empty));
	}

	[AllowAnonymous]
	[HttpPatch("token/refresh")]
	public async Task<ActionResult<RefreshTokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto reqDto)
	{
		return Ok(await authService.RefreshToken(reqDto));
	}

	[HttpDelete("logout")]
	public async Task<ActionResult<MessageContentResDto>> Logout()
	{
		var refreshToken = Request.Headers["X-RefreshToken"][0] ?? "";
		return Ok(await authService.Logout(refreshToken, HttpContext.User));
	}
}
