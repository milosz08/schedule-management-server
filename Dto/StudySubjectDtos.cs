using System.ComponentModel.DataAnnotations;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class StudySubjectRequestDto
    {
        [Required(ErrorMessage = "Pole nazwy przedmiotu nie może być puste")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Pole nazwy wydziału nie może być puste")]
        public string DepartmentName { get; set; }
        
        [Required(ErrorMessage = "Pole nazwy kierunku nie może być puste")]
        public string StudySpecName { get; set; }
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class StudySubjectResponseDto
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string DepartmentFullName { get; set; }
        public string StudySpecFullName { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class StudySubjectQueryResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string SpecName { get; set; }
        public string SpecAlias { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentAlias { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class StudySubjectEditResDto
    {
        public string Name { get; set; }
        public string DepartmentName { get; set; }
        public string StudySpecName { get; set; }
    }
}