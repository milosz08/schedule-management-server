using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Api.Attribute;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Network.Auth;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.User;

[ApiController]
[Route("/api/v1/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController(IUserService userService) : ControllerBase
{
	[HttpGet("all/pageable")]
	[AuthorizeRoles(UserRole.Administrator)]
	public ActionResult<UserResponseDto> GetAllUsers([FromQuery] SearchQueryRequestDto searchQuery)
	{
		return Ok(userService.GetAllUsers(searchQuery));
	}

	[AllowAnonymous]
	[HttpGet("employeer/dept/{deptId:long}/cath/{cathId:long}/all")]
	public async Task<ActionResult<List<NameIdElementDto>>> GetAllEmployeersScheduleBaseCath([FromRoute] long deptId,
		[FromRoute] long cathId)
	{
		return Ok(await userService.GetAllEmployeersScheduleBaseCath(deptId, cathId));
	}

	[AllowAnonymous]
	[HttpGet("teacher/dept/{deptId:long}/all")]
	public async Task<ActionResult<List<NameIdElementDto>>> GetAllTeachersScheduleBaseDeptAndSpec(
		[FromRoute] long deptId,
		[FromQuery] string subjectName)
	{
		return Ok(await userService.GetAllTeachersScheduleBaseDeptAndSpec(deptId, subjectName));
	}

	[HttpGet("dashboard/details")]
	public async Task<ActionResult<DashboardDetailsResDto>> GetDashboardPanelData()
	{
		return Ok(await userService.GetDashboardPanelData(HttpContext.User));
	}

	[HttpGet("{userId:long}/details")]
	[AuthorizeRoles(UserRole.Administrator)]
	public async Task<ActionResult<UserDetailsEditResDto>> GetUserDetails([FromRoute] long userId)
	{
		return Ok(await userService.GetUserDetails(userId));
	}

	[HttpPut("{userId:long}")]
	[AuthorizeRoles(UserRole.Administrator)]
	public async Task<ActionResult<RegisterUpdateUserResponseDto>> UpdateUserDetails(
		[FromBody] RegisterUpdateUserRequestDto dto,
		[FromRoute] long userId,
		[FromQuery] bool isUpdateEmailPass)
	{
		return Ok(await userService.UpdateUserDetails(dto, userId, isUpdateEmailPass));
	}

	[HttpDelete("selected")]
	[AuthorizeRoles(UserRole.Administrator)]
	public async Task<ActionResult> DeleteMassiveUsers([FromBody] DeleteSelectedRequestDto users)
	{
		return Ok(await userService.Delete2WayFactorSelected(users, HttpContext, Request));
	}

	[HttpDelete("all")]
	[AuthorizeRoles(UserRole.Administrator)]
	public async Task<ActionResult> DeleteAllUsers()
	{
		return Ok(await userService.Delete2WayFactorAll(HttpContext, Request));
	}
}
