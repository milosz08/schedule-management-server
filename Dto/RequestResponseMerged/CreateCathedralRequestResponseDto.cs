using System.ComponentModel.DataAnnotations;


namespace asp_net_po_schedule_management_server.Dto.RequestResponseMerged
{
    public class CreateCathedralRequestDto
    {
        [Required(ErrorMessage = "Pole nazwy katedry nie może być puste")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Pole aliasu katedry nie może być puste")]
        public string Alias { get; set; }
        
        [Required(ErrorMessage = "Pole przypisanego wydziału do katedry nie może być puste")]
        public string DepartmentName { get; set; }
    }

    public sealed class CreatedCathedralResponseDto : CreateCathedralRequestDto
    {
        public string DepartmentAlias { get; set; }
    }
}