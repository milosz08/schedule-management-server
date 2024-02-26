namespace ScheduleManagement.Api.Network.Auth;

public sealed class RefreshTokenRequestDto
{
	public string ExpiredAccessToken { get; set; }
	public string RefreshToken { get; set; }
}

public sealed class RefreshTokenResponseDto
{
	public string AccessToken { get; set; }
	public string RefreshToken { get; set; }
}
