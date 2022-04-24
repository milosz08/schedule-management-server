using System.ComponentModel.DataAnnotations;


namespace asp_net_po_schedule_management_server.Dto.Requests
{
    public sealed class RegisterNewUserRequestDto
    {
        [Required(ErrorMessage = "Imię nie może być puste")]
        [MinLength(3, ErrorMessage = "Imię musi być dłuższe od 3 znaków")]
        [MaxLength(50, ErrorMessage = "Imię musi być krótsze od 50 znaków")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Nazwisko nie może być puste")]
        [MinLength(3, ErrorMessage = "Nazwisko musi być dłuższe od 3 znaków")]
        [MaxLength(50, ErrorMessage = "Nazwisko musi być krótsze od 50 znaków")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Narowodowść nie może być pusta")]
        [MinLength(3, ErrorMessage = "Narowodowść musi być dłuższa od 3 znaków")]
        [MaxLength(100, ErrorMessage = "Narowodowść musi być krótsza od 100 znaków")]
        public string Nationality { get; set; }

        [MinLength(3, ErrorMessage = "Miasto musi być dłuższe od 3 znaków")]
        [MaxLength(100, ErrorMessage = "Miasto musi być krótsze od 100 znaków")]
        public string City { get; set; }
    }
}