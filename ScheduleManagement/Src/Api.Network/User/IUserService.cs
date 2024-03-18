using System.Security.Claims;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Network.Auth;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.User;

public interface IUserService : IBaseCrudService
{
	PaginationResponseDto<UserResponseDto> GetAllUsers(SearchQueryRequestDto searchQuery);

	Task<RegisterUpdateUserResponseDto> UpdateUserDetails(RegisterUpdateUserRequestDto dto, long userId,
		bool isUpdateEmailPass);

	Task<List<NameIdElementDto>> GetAllEmployeersScheduleBaseCath(long deptId, long cathId);

	Task<List<NameIdElementDto>> GetAllTeachersScheduleBaseDeptAndSpec(long deptId, string subjectName);

	Task<DashboardDetailsResDto> GetDashboardPanelData(ClaimsPrincipal claimsPrincipal);

	Task<UserDetailsEditResDto> GetUserDetails(long userId);
}
