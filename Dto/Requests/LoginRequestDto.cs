using System.ComponentModel.DataAnnotations;


namespace asp_net_po_schedule_management_server.Dto.Requests
{
    public sealed class LoginRequestDto
    {
        [Required(ErrorMessage = "Pole loginu nie może być puste")]
        public string Login { get; set; }
        
        [Required(ErrorMessage = "Pole hasła nie może być puste")]
        public string Password { get; set; }
    }
}