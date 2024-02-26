using System.Security.Claims;
using ScheduleManagement.Api.Dto;

namespace ScheduleManagement.Api.Network.Auth;

public interface IAuthService
{
	Task<LoginResponseDto> Login(LoginRequestDto reqDto);

	Task<LoginResponseDto> TokenLogin(TokenLoginRequestDto reqDto);

	Task<RefreshTokenResponseDto> RefreshToken(RefreshTokenRequestDto reqDto);

	Task<RegisterUpdateUserResponseDto> Register(RegisterUpdateUserRequestDto reqDto, string customPassword);

	Task<MessageContentResDto> Logout(string refreshToken, ClaimsPrincipal claimsPrincipal);
}
