using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.StudySpec;

public interface IStudySpecService : IBaseCrudService
{
	Task<IEnumerable<StudySpecResponseDto>> CreateStudySpecialization(StudySpecRequestDto dto);

	Task<List<StudySpecResponseDto>> UpdateStudySpecialization(StudySpecRequestDto dto, long specId);

	Task<SearchQueryResponseDto> GetAllStudySpecializationsInDepartment(string? specName, string? deptName);

	PaginationResponseDto<StudySpecQueryResponseDto> GetAllStudySpecializations(SearchQueryRequestDto searchQuery);

	Task<List<NameIdElementDto>> GetAllStudySpecsScheduleBaseDept(long deptId, long degreeId);

	Task<AvailableDataResponseDto<NameIdElementDto>> GetAvailableStudySpecsBaseDept(string deptName);

	Task<StudySpecializationEditResDto> GetStudySpecializationDetails(long specId);
}
