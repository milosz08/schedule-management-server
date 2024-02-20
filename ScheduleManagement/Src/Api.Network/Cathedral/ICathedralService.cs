using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.Cathedral;

public interface ICathedralService : IBaseCrudService
{
	Task<CathedralResponseDto> CreateCathedral(CathedralRequestDto dto);

	Task<CathedralResponseDto> UpdateCathedral(CathedralRequestDto dto, long cathId);

	SearchQueryResponseDto GetAllCathedralsBasedDepartmentName(string? deptName, string? cathName);

	PaginationResponseDto<CathedralQueryResponseDto> GetAllCathedrals(SearchQueryRequestDto searchQuery);

	List<NameIdElementDto> GetAllCathedralsScheduleBaseDept(long deptId);

	Task<CathedralEditResDto> GetCathedralDetails(long cathId);
}