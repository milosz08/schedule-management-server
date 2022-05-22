using System.Collections.Generic;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class CreateStudyGroupRequestDto
    {
        public string DepartmentName { get; set; }
        public string StudySpecName { get; set; }
        public List<long> Semesters { get; set; }
        public int CountOfGroups { get; set; }
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class CreateStudyGroupResponseDto
    {
        public string Name { get; set; }
        public string DepartmentFullName { get; set; }
        public string StudySpecFullName { get; set; }
        public string SemesterName { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class StudyGroupQueryResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentAlias { get; set; }
        public string StudySpecName { get; set; }
        public string StudySpecAlias { get; set; }
    }
}