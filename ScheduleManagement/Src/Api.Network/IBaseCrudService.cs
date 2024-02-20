using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network;

public interface IBaseCrudService
{
	Task<MessageContentResDto> Delete2WayFactorSelected(DeleteSelectedRequestDto items, HttpContext context,
		HttpRequest request);

	Task<MessageContentResDto> Delete2WayFactorAll(HttpContext context, HttpRequest request);
}
