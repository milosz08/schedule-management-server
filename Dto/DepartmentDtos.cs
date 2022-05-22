using System.ComponentModel.DataAnnotations;


namespace asp_net_po_schedule_management_server.Dto
{
    public class DepartmentRequestResponseDto
    {
        [Required(ErrorMessage = "Pole nazwy wydziału nie może być puste")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Pole aliasu nazwy wydziału nie może być puste")]
        public string Alias { get; set; }
    }
}