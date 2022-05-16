using System.ComponentModel.DataAnnotations;


namespace asp_net_po_schedule_management_server.Dto.RequestResponseMerged
{
    public class StudySpecRequestDto
    {
        [Required(ErrorMessage = "Pole nazwy kierunku nie może być puste")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Pole aliasu nazwy kierunku nie może być puste")]
        public string Alias { get; set; }
        
        [Required(ErrorMessage = "Pole nazwy przypisanego wydziału do kierunku nie może być puste")]
        public string DepartmentName { get; set; }
        
        [Required(ErrorMessage = "Pole typu kierunku do kierunku nie może być puste")]
        public string StudyType { get; set; }
    }

    public sealed class StudySpecResponseDto : StudySpecRequestDto
    {
        public string DepartmentAlias { get; set; }
        public string StudyTypeFullName { get; set; }
    }
}