using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.StudySubject;

public interface IStudySubjectService : IBaseCrudService
{
	Task<StudySubjectResponseDto> CreateStudySubject(StudySubjectRequestDto dto);

	Task<StudySubjectResponseDto> UpdateStudySubject(StudySubjectRequestDto dto, long subjId);

	PaginationResponseDto<StudySubjectQueryResponseDto> GetAllStudySubjects(SearchQueryRequestDto searchQuery);

	Task<SearchQueryResponseDto> GetAllStudySubjectsBaseDeptAndSpec(string? subjcName, long deptId, long studySpecId);

	Task<AvailableDataResponseDto<NameIdElementDto>> GetAvailableSubjectsBaseDept(string deptName);

	Task<StudySubjectEditResDto> GetStudySubjectBaseDbId(long subjId);
}
