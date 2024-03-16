using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.Department;

public interface IDepartmentService : IBaseCrudService
{
	Task<DepartmentRequestResponseDto> CreateDepartment(DepartmentRequestResponseDto dto);

	Task<DepartmentRequestResponseDto> UpdateDepartment(DepartmentRequestResponseDto dto, long deptId);

	Task<SearchQueryResponseDto> GetAllDepartments(string? name);

	PaginationResponseDto<DepartmentQueryResponseDto> GetPageableDepartments(SearchQueryRequestDto searchQuery);

	Task<List<NameIdElementDto>> GetAllDepartmentsSchedule();

	Task<DepartmentEditResDto> GetDepartmentDetails(long deptId);
}
