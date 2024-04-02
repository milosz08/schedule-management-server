using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Api.Attribute;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Network.User;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.LastOpenedSchedules;

[ApiController]
[Route("/api/v1/[controller]")]
[AuthorizeRoles(UserRole.Administrator, UserRole.Editor)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LastOpenedSchedulesController(ILastOpenedSchedulesService lastOpenedSchedulesService) : ControllerBase
{
	[HttpGet("all")]
	public async Task<ActionResult<List<LastOpenedScheduleData>>> GetAllLastOpenedSchedules()
	{
		return Ok(await lastOpenedSchedulesService.GetAllLastOpenedSchedules(HttpContext.User));
	}

	[HttpPost]
	public async Task<ActionResult<LastOpenedScheduleResponseDto>> AppendLastOpenedSchedule(
		[FromBody] LastOpenedScheduleRequestDto requestRequestDto)
	{
		return Created(string.Empty, await lastOpenedSchedulesService
			.AppendLastOpenedSchedule(requestRequestDto, HttpContext.User));
	}

	[HttpDelete("selected")]
	public async Task<ActionResult<MessageContentResDto>> DeleteSelectedLastOpenedSchedules(
		[FromBody] DeleteSelectedRequestDto requestDto)
	{
		return Ok(await lastOpenedSchedulesService.DeleteSelectedLastOpenedSchedules(requestDto, HttpContext.User));
	}

	[HttpDelete("all")]
	public async Task<ActionResult<MessageContentResDto>> DeleteAllLastOpenedSchedules()
	{
		return Ok(await lastOpenedSchedulesService.DeleteAllLastOpenedSchedules(HttpContext.User));
	}
}
