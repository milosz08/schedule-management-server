using System.Security.Claims;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.ContactMessage;

public interface IContactMessageService : IBaseCrudService
{
	Task<MessageContentResDto> CreateMessage(ContactMessagesReqDto dto, ClaimsPrincipal claimsPrincipal);

	Task<AvailableDataResponseDto<string>> GetAllContactMessageIssueTypes(string? issueTypeName);

	Task<PaginationResponseDto<ContactMessagesQueryResponseDto>> GetAllMessagesBaseClaims(
		SearchQueryRequestDto searchQuery, ClaimsPrincipal claimsPrincipal);

	Task<SingleContactMessageResponseDto> GetContactMessageDetails(long messId, ClaimsPrincipal claimsPrincipal);
}