using asp_net_po_schedule_management_server.Entities;

namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class ConvertNamesToIdsRequestDto
    {
        public string DepartmentName { get; set; }
        public string StudySpecName { get; set; }
        public string StudyGroupName { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class ConvertIdsToNamesRequestDto
    {
        public long? DepartmentId { get; set; }
        public long? StudySpecId { get; set; }
        public long? StudyGroupId { get; set; }
    }

    //------------------------------------------------------------------------------------------------------------------

    public sealed class ConvertNamesToDataSingleElement
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public ConvertNamesToDataSingleElement(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public sealed class ConvertToNameWithIdResponseDto
    {
        public ConvertNamesToDataSingleElement DeptData { get; set; }
        public ConvertNamesToDataSingleElement StudySpecData { get; set; }
        public ConvertNamesToDataSingleElement StudyGroupData { get; set; }
        
        public ConvertToNameWithIdResponseDto(Department deptData, StudySpecialization specData, StudyGroup groupData)
        {
            DeptData = new ConvertNamesToDataSingleElement(deptData.Id, deptData.Name.ToLower());
            StudySpecData = new ConvertNamesToDataSingleElement(specData.Id, specData.Name.ToLower());
            StudyGroupData = new ConvertNamesToDataSingleElement(groupData.Id, groupData.Name.ToLower());
        }
    }
}