using System.ComponentModel.DataAnnotations;

namespace ScheduleManagement.Api.Network.Auth;

public sealed class LoginRequestDto
{
	[Required(ErrorMessage = "Pole loginu nie może być puste.")]
	public string Login { get; set; }

	[Required(ErrorMessage = "Pole hasła nie może być puste.")]
	public string Password { get; set; }
}

public sealed class TokenLoginRequestDto
{
	[Required(ErrorMessage = "Pole tokenu nie może być puste.")]
	public string AccessToken { get; set; }

	[Required(ErrorMessage = "Pole tokenu odświeżania nie może być puste.")]
	public string RefreshToken { get; set; }
}

public sealed class LoginResponseDto
{
	public long Id { get; set; }
	public string BearerToken { get; set; }
	public string RefreshBearerToken { get; set; }
	public string Role { get; set; }
	public string NameWithSurname { get; set; }
	public string Login { get; set; }
	public string Email { get; set; }
	public bool FirstAccess { get; set; }
	public string? ProfileImageUrl { get; set; }
	public string ConnectedWithDepartment { get; set; }
}
