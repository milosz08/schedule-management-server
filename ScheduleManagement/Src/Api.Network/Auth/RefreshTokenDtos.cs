namespace ScheduleManagement.Api.Network.Auth;

public sealed class RefreshTokenRequestDto
{
	public string BearerToken { get; set; }
	public string RefreshBearerToken { get; set; }
}

public sealed class RefreshTokenResponseDto
{
	public string BearerToken { get; set; }
	public string RefreshBearerToken { get; set; }
	public DateTime TokenExpirationDate { get; set; }
	public double TokenRefreshInSeconds { get; set; }
}
