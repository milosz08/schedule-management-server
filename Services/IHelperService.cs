using System.Threading.Tasks;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IHelperService
    {
        AvailableStudyTypesResponseDto GetAvailableStudyTypes();
        AvailablePaginationSizes GetAvailablePaginationTypes();
        AvailableRoomTypesResponseDto GetAvailableRoomTypes();
    }
}