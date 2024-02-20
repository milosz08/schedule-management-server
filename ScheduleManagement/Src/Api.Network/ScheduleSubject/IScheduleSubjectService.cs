using ScheduleManagement.Api.Dto;

namespace ScheduleManagement.Api.Network.ScheduleSubject;

public interface IScheduleSubjectService : IBaseCrudService
{
	Task<MessageContentResDto> AddNewScheduleActivity(ScheduleActivityReqDto dto);

	Task<ScheduleDataRes<ScheduleGroups>> GetAllScheduleSubjectsBaseGroup(ScheduleGroupQuery dto,
		ScheduleFilteringData filter);

	Task<ScheduleDataRes<ScheduleTeachers>> GetAllScheduleSubjectsBaseTeacher(ScheduleTeacherQuery dto,
		ScheduleFilteringData filter);

	Task<ScheduleDataRes<ScheduleRooms>> GetAllScheduleSubjectsBaseRoom(ScheduleRoomQuery dto,
		ScheduleFilteringData filter);

	Task<ScheduleSubjectDetailsResDto> GetScheduleSubjectDetails(long schedSubjId);
}
