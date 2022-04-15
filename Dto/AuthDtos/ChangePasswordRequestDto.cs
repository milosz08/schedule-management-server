using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;


namespace asp_net_po_schedule_management_server.Dto.AuthDtos
{
    public sealed class ChangePasswordRequestDto
    {
        [JsonPropertyName("oldPassword")]
        [Required(ErrorMessage = "Pole poprzedniego hasła nie może być puste")]
        public string OldPassword { get; set; }

        [JsonPropertyName("newPassword")]
        [Required(ErrorMessage = "Pole nowego hasła nie może być puste")]
        public string NewPassword { get; set; }

        [JsonPropertyName("newPasswordConfirmed")]
        [Required(ErrorMessage = "Pole potwierdzenia nowego hasła nie może być puste")]
        [Compare("NewPassword", ErrorMessage = "Hasła w obu polach muszą być identyczne.")]
        public string NewPasswordConfirmed { get; set; }
    }
}