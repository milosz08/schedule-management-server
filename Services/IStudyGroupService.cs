using System.Threading.Tasks;
using System.Collections.Generic;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IStudyGroupService
    {
        Task<List<CreateStudyGroupResponseDto>> CreateStudyGroup(CreateStudyGroupRequestDto dto);
        PaginationResponseDto<StudyGroupQueryResponseDto> GetAllStudyGroups(SearchQueryRequestDto searchQuery);
        Task<List<NameWithDbIdElement>> GetAvailableGroupsBaseStudySpecAndSem(long studySpecId, long semId);
        Task<SearchQueryResponseDto> GetGroupsBaseStudySpec(string groupName, string deptName, string studySpecName);
        Task<List<NameWithDbIdElement>> GetAllStudyGroupsBaseDept(string deptName);
        Task DeleteMassiveStudyGroups(MassiveDeleteRequestDto studyGroups, UserCredentialsHeaderDto credentials);
        Task DeleteAllStudyGroups(UserCredentialsHeaderDto credentials);
    }
}