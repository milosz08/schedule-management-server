using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.StudyRoom;

public interface IStudyRoomService : IBaseCrudService
{
	Task<StudyRoomResponseDto> CreateStudyRoom(StudyRoomRequestDto dto);

	Task<StudyRoomResponseDto> UpdateStudyRoom(StudyRoomRequestDto dto, long roomId);

	PaginationResponseDto<StudyRoomQueryResponseDto> GetAllStudyRooms(SearchQueryRequestDto searchQuery);

	Task<List<NameIdElementDto>> GetAllStudyRoomsScheduleBaseCath(long deptId, long cathId);

	Task<List<NameIdElementDto>> GetAllStudyRoomsScheduleBaseDeptName(long deptId);

	Task<StudyRoomEditResDto> GetStudyRoomDetails(long roomId);
}