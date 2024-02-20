using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Api.Attribute;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Network.User;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.ScheduleSubject;

[ApiController]
[Route("/api/v1/[controller]")]
[AuthorizeRoles(UserRole.Editor, UserRole.Administrator)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ScheduleSubjectController(IScheduleSubjectService scheduleSubjectService)
	: ControllerBase
{
	[AllowAnonymous]
	[HttpGet("{schedSubjId:long}/details")]
	public async Task<ActionResult<ScheduleSubjectDetailsResDto>> GetScheduleSubjectDetails(
		[FromRoute] long schedSubjId)
	{
		return Ok(await scheduleSubjectService.GetScheduleSubjectDetails(schedSubjId));
	}

	[HttpPost]
	public async Task<ActionResult<MessageContentResDto>> AddNewScheduleActivity([FromBody] ScheduleActivityReqDto dto)
	{
		return Created(string.Empty, await scheduleSubjectService.AddNewScheduleActivity(dto));
	}

	[AllowAnonymous]
	[HttpPost("all/filter/group")]
	public async Task<ActionResult<ScheduleDataRes<ScheduleGroups>>> GetAllScheduleSubjectsBaseGroup(
		[FromQuery] ScheduleGroupQuery dto,
		[FromBody] ScheduleFilteringData filter)
	{
		return Ok(await scheduleSubjectService.GetAllScheduleSubjectsBaseGroup(dto, filter));
	}

	[AllowAnonymous]
	[HttpPost("all/filter/teacher")]
	public async Task<ActionResult<ScheduleDataRes<ScheduleTeachers>>> GetAllScheduleSubjectsBaseTeacher(
		[FromQuery] ScheduleTeacherQuery dto,
		[FromBody] ScheduleFilteringData filter)
	{
		return Ok(await scheduleSubjectService.GetAllScheduleSubjectsBaseTeacher(dto, filter));
	}

	[AllowAnonymous]
	[HttpPost("all/filter/room")]
	public async Task<ActionResult<ScheduleDataRes<ScheduleRooms>>> GetAllScheduleSubjectsBaseRoom(
		[FromQuery] ScheduleRoomQuery dto,
		[FromBody] ScheduleFilteringData filter)
	{
		return Ok(await scheduleSubjectService.GetAllScheduleSubjectsBaseRoom(dto, filter));
	}

	[HttpDelete("selected")]
	public async Task<ActionResult<MessageContentResDto>> DeleteSelectedScheduleSubjects(
		[FromBody] DeleteSelectedRequestDto scheduleSubjects)
	{
		return Ok(await scheduleSubjectService.Delete2WayFactorSelected(scheduleSubjects, HttpContext, Request));
	}
}
