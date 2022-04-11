using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;


namespace asp_net_po_schedule_management_server.Dto
{
    public class UserRequestDto
    {
        [JsonPropertyName("login")]
        [Required(ErrorMessage = "Pole loginu nie może być puste")]
        public string Login { get; set; }
        
        [JsonPropertyName("password")]
        [Required(ErrorMessage = "Pole hasła nie może być puste")]
        public string Password { get; set; }
    }
}