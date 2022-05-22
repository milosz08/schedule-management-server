using System.Threading.Tasks;
using System.Collections.Generic;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IUsersService
    {
        PaginationResponseDto<UserResponseDto> GetAllUsers(SearchQueryRequestDto searchQuery);
        List<NameWithDbIdElement> GetAllEmployeersScheduleBaseCath(long deptId, long cathId);
        Task DeleteMassiveUsers(MassiveDeleteRequestDto users, UserCredentialsHeaderDto credentials);
        Task DeleteAllUsers(UserCredentialsHeaderDto credentials);
    }
}