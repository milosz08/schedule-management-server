using System.Security.Claims;
using System.Threading.Tasks;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IContactMessagesService
    {
        Task<PseudoNoContentResponseDto> AddNewMessage(ContactMessagesReqDto dto, Claim userIdentity);
        Task<AvailableDataResponseDto<string>> GetAllContactMessageIssueTypes(string issueTypeName);
        Task<PaginationResponseDto<ContactMessagesQueryResponseDto>> GetAllMessagesBaseClaims(
            SearchQueryRequestDto searchQuery, Claim userRole, Claim userLogin);
        Task<SingleContactMessageResponseDto> GetContactMessageBaseId(long messId, Claim userRole, Claim userLogin);
        Task DeleteMassiveContactMess(MassiveDeleteRequestDto dto, UserCredentialsHeaderDto credentials);
        Task DeleteAllContactMess(UserCredentialsHeaderDto credentials);
    }
}