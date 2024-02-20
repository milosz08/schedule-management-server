using System.ComponentModel.DataAnnotations;
using ScheduleManagement.Api.Util;

namespace ScheduleManagement.Api.Network.ResetPassword;

public sealed class ChangePasswordRequestDto
{
	[Required(ErrorMessage = "Pole poprzedniego hasła nie może być puste")]
	public string OldPassword { get; set; }

	[Required(ErrorMessage = "Pole nowego hasła nie może być puste")]
	[RegularExpression(Regex.Password,
		ErrorMessage = "Hasło musi mieć minimum 8 znaków, zawierać co najmniej jedną liczbę, " +
		               "jedną wielką literę oraz jeden znak specjalny.")]
	public string NewPassword { get; set; }

	[Required(ErrorMessage = "Pole potwierdzenia nowego hasła nie może być puste")]
	[Compare(nameof(NewPassword), ErrorMessage = "Hasła w obu polach muszą być identyczne.")]
	public string NewPasswordConfirmed { get; set; }
}

public sealed class SetResetPasswordRequestDto
{
	[Required(ErrorMessage = "Pole nowego hasła nie może być puste")]
	[RegularExpression(Regex.Password,
		ErrorMessage = "Hasło musi mieć minimum 8 znaków, zawierać co najmniej jedną liczbę, " +
		               "jedną wielką literę oraz jeden znak specjalny.")]
	public string NewPassword { get; set; }

	[Required(ErrorMessage = "Pole potwierdzenia nowego hasła nie może być puste")]
	[Compare(nameof(NewPassword), ErrorMessage = "Hasła w obu polach muszą być identyczne.")]
	public string NewPasswordConfirmed { get; set; }
}

public sealed class SetNewPasswordViaEmailResponse
{
	public string Email { get; set; }
	public string Token { get; set; }
}
