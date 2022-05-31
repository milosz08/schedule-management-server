using System.Threading.Tasks;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IScheduleSubjectsService
    {
        Task AddNewScheduleActivity(ScheduleActivityReqDto dto);
        Task<ScheduleDataRes<ScheduleGroups>> GetAllScheduleSubjectsBaseGroup(ScheduleGroupQuery dto, ScheduleFilteringData filter);
        Task<ScheduleDataRes<ScheduleTeachers>> GetAllScheduleSubjectsBaseTeacher(ScheduleTeacherQuery dto, ScheduleFilteringData filter);
        Task<ScheduleDataRes<ScheduleRooms>> GetAllScheduleSubjectsBaseRoom(ScheduleRoomQuery dto, ScheduleFilteringData filter);
        Task<ScheduleSubjectDetailsResDto> GetScheduleSubjectDetails(long schedSubjId);
        Task DeleteMassiveScheduleSubjects(MassiveDeleteRequestDto scheduleSubjects, UserCredentialsHeaderDto credentials);
    }
}