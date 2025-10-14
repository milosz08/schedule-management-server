using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.StudyGroup;

public interface IStudyGroupService : IBaseCrudService
{
	Task<List<CreateStudyGroupResponseDto>> CreateStudyGroup(CreateStudyGroupRequestDto dto);

	PaginationResponseDto<StudyGroupQueryResponseDto> GetAllStudyGroups(SearchQueryRequestDto searchQuery);

	Task<List<NameIdElementDto>> GetAvailableGroupsBaseStudySpecAndSem(long studySpecId, long semId);

	Task<SearchQueryResponseDto> GetGroupsBaseStudySpec(string? groupName, string? deptName, string? studySpecName);

	Task<List<NameIdElementDto>> GetAllStudyGroupsBaseDept(string deptName);
}
