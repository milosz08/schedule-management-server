using Microsoft.AspNetCore.Mvc;

namespace ScheduleManagement.Api.Network.TimeManagement;

[ApiController]
[Route("/api/v1/[controller]")]
public class TimeManagementController(ITimeManagementService timeManagementService) : ControllerBase
{
	[HttpGet("study/years")]
	public ActionResult<List<string>> GetAllStudyYearsFrom2020ToCurrent()
	{
		return Ok(timeManagementService.GetAllStudyYearsFrom2020ToCurrent());
	}

	[HttpGet("weekdata/from/year/{fromYear:int}/to/year/{toYear:int}")]
	public ActionResult<List<string>> GetAllWeeksNameWithWeekNumberInCurrentYear([FromRoute] int fromYear,
		[FromRoute] int toYear)
	{
		return Ok(timeManagementService.GetAllWeeksNameWithWeekNumberInCurrentYear(fromYear, toYear));
	}
}
