namespace ScheduleManagement.Api.Network.TimeManagement;

public interface ITimeManagementService
{
	List<string> GetAllStudyYearsFrom2020ToCurrent();

	List<string> GetAllWeeksNameWithWeekNumberInCurrentYear(int startYear, int endYear);
}
