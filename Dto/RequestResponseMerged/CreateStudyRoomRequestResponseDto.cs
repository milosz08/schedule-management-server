using System.ComponentModel.DataAnnotations;


namespace asp_net_po_schedule_management_server.Dto.RequestResponseMerged
{
    public class CreateStudyRoomRequestDto
    {
        [Required(ErrorMessage = "Pole nazwy (aliasu) sali nie może być puste")]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Pole nazwy wydziału nie może być puste")]
        public string DepartmentName { get; set; }
        
        [Required(ErrorMessage = "Pole nazwy katedry nie może być puste")]
        public string CathedralName { get; set; }
        
        [Required(ErrorMessage = "Pole pojemności sali nie może być puste")]
        public int Capacity { get; set; }
        
        [Required(ErrorMessage = "Pole typu sali nie może być puste")]
        public string RoomType { get; set; }
    }

    public sealed class CreateStudyRoomResponseDto : CreateStudyRoomRequestDto
    {
        public string DepartmentAlias { get; set; }
        public string CathedralAlias { get; set; }
        public string RoomTypeName { get; set; }
    }
}