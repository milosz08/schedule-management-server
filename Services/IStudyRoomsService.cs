using System.Threading.Tasks;

using asp_net_po_schedule_management_server.Dto.RequestResponseMerged;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IStudyRoomsService
    {
        Task<CreateStudyRoomResponseDto> CreateStudyRoom(CreateStudyRoomRequestDto dto);
    }
}