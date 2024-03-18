using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Api.Attribute;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Network.User;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.StudyGroup;

[ApiController]
[Route("/api/v1/[controller]")]
[AuthorizeRoles(UserRole.Administrator)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class StudyGroupController(IStudyGroupService studyGroupService) : ControllerBase
{
	[AllowAnonymous]
	[HttpGet("spec/{specId:long}/sem/{semId:long}")]
	public async Task<ActionResult<List<NameIdElementDto>>> GetAvailableStudyGroupsBaseSpecAndSem(
		[FromRoute] long specId,
		[FromRoute] long semId)
	{
		return Ok(await studyGroupService.GetAvailableGroupsBaseStudySpecAndSem(specId, semId));
	}

	[AllowAnonymous]
	[HttpGet("spec")]
	public async Task<ActionResult<SearchQueryResponseDto>> GetAvailableStudyGroupsBaseSpec(
		[FromQuery] string? groupName,
		[FromQuery] string? deptName,
		[FromQuery] string? studySpecName)
	{
		return Ok(await studyGroupService.GetGroupsBaseStudySpec(groupName, deptName, studySpecName));
	}

	[AllowAnonymous]
	[HttpGet("dept")]
	public async Task<ActionResult<List<NameIdElementDto>>> GetAllStudyGroupsBaseDept([FromQuery] string dept)
	{
		return Ok(await studyGroupService.GetAllStudyGroupsBaseDept(dept));
	}

	[HttpGet("all/pageable")]
	public ActionResult<StudyGroupQueryResponseDto> GetStudyRooms([FromQuery] SearchQueryRequestDto searchQuery)
	{
		return Ok(studyGroupService.GetAllStudyGroups(searchQuery));
	}

	[HttpPost]
	public async Task<ActionResult<List<CreateStudyGroupResponseDto>>> CreateStudyGroup(
		[FromBody] CreateStudyGroupRequestDto dto)
	{
		return Created(string.Empty, await studyGroupService.CreateStudyGroup(dto));
	}

	[HttpDelete("selected")]
	public async Task<ActionResult<MessageContentResDto>> DeleteSelectedGroups(
		[FromBody] DeleteSelectedRequestDto groups)
	{
		return Ok(await studyGroupService.Delete2WayFactorSelected(groups, HttpContext, Request));
	}

	[HttpDelete("all")]
	public async Task<ActionResult> DeleteAllGroups()
	{
		return Ok(await studyGroupService.Delete2WayFactorAll(HttpContext, Request));
	}
}
