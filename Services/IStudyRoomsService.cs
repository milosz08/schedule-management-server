using System.Threading.Tasks;
using System.Collections.Generic;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IStudyRoomsService
    {
        Task<StudyRoomResponseDto> CreateStudyRoom(StudyRoomRequestDto dto);
        Task<StudyRoomResponseDto> UpdateStudyRoom(StudyRoomRequestDto dto, long roomId);
        PaginationResponseDto<StudyRoomQueryResponseDto> GetAllStudyRooms(SearchQueryRequestDto searchQuery);
        Task<List<NameWithDbIdElement>> GetAllStudyRoomsScheduleBaseCath(long deptId, long cathId);
        Task<List<NameWithDbIdElement>> GetAllStudyRoomsScheduleBaseDeptName(long deptId);
        Task<StudyRoomEditResDto> GetStudyRoomBaseDbId(long roomId);
        Task DeleteMassiveStudyRooms(MassiveDeleteRequestDto studyRooms, UserCredentialsHeaderDto credentials);
        Task DeleteAllStudyRooms(UserCredentialsHeaderDto credentials);
    }
}