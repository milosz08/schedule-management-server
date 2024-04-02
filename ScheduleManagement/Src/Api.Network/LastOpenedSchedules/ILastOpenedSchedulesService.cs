using System.Security.Claims;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.LastOpenedSchedules;

public interface ILastOpenedSchedulesService
{
	Task<List<LastOpenedScheduleData>> GetAllLastOpenedSchedules(ClaimsPrincipal claimsPrincipal);

	Task<LastOpenedScheduleResponseDto> AppendLastOpenedSchedule(LastOpenedScheduleRequestDto requestDto,
		ClaimsPrincipal claimsPrincipal);

	Task<MessageContentResDto> DeleteSelectedLastOpenedSchedules(DeleteSelectedRequestDto requestDto,
		ClaimsPrincipal claimsPrincipal);

	Task<MessageContentResDto> DeleteAllLastOpenedSchedules(ClaimsPrincipal claimsPrincipal);
}
