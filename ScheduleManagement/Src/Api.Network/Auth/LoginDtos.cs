using System.ComponentModel.DataAnnotations;

namespace ScheduleManagement.Api.Network.Auth;

public sealed class LoginRequestDto
{
    [Required(ErrorMessage = "Pole loginu nie może być puste")]
    public string Login { get; set; }

    [Required(ErrorMessage = "Pole hasła nie może być puste")]
    public string Password { get; set; }
}

public sealed class LoginResponseDto
{
    public string DictionaryHash { get; set; }
    public string BearerToken { get; set; }
    public string RefreshBearerToken { get; set; }
    public string Role { get; set; }
    public string NameWithSurname { get; set; }
    public string Login { get; set; }
    public string Email { get; set; }
    public DateTime TokenExpirationDate { get; set; }
    public double TokenRefreshInSeconds { get; set; }
    public bool FirstAccess { get; set; }
    public bool HasPicture { get; set; }
    public string ConnectedWithDepartment { get; set; }
}
