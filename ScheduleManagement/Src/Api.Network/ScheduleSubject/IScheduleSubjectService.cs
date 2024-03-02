using ScheduleManagement.Api.Dto;

namespace ScheduleManagement.Api.Network.ScheduleSubject;

public interface IScheduleSubjectService : IBaseCrudService
{
	Task<MessageContentResDto> AddNewScheduleActivity(ScheduleActivityReqDto dto);

	Task<ScheduleDataRes> GetAllScheduleSubjectsBaseGroup(ScheduleGroupQuery dto, ScheduleFilteringData filter);

	Task<ScheduleDataRes> GetAllScheduleSubjectsBaseEmployer(ScheduleEmployerQuery dto, ScheduleFilteringData filter);

	Task<ScheduleDataRes> GetAllScheduleSubjectsBaseRoom(ScheduleRoomQuery dto, ScheduleFilteringData filter);

	Task<ScheduleSubjectDetailsResDto> GetScheduleSubjectDetails(long schedSubjId);
}
