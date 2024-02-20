using System.Security.Claims;
using ScheduleManagement.Api.Dto;

namespace ScheduleManagement.Api.Network.Auth;

public interface IAuthService
{
	Task<LoginResponseDto> Login(LoginRequestDto user);

	Task<RefreshTokenResponseDto> RefreshToken(RefreshTokenRequestDto dto);

	Task<RegisterUpdateUserResponseDto> Register(RegisterUpdateUserRequestDto user, string customPassword);

	Task<MessageContentResDto> Logout(string refreshToken, ClaimsPrincipal claimsPrincipal);
}