using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Api.Attribute;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Network.User;

namespace ScheduleManagement.Api.Network.Helper;

[ApiController]
[Route("/api/v1/[controller]")]
public class HelperController(IHelperService helperService) : ControllerBase
{
	[HttpGet("study/types/all")]
	public async Task<ActionResult<AvailableDataResponseDto<NameIdElementDto>>> GetAvailableStudyTypes()
	{
		return Ok(await helperService.GetAvailableStudyTypes());
	}

	[HttpGet("study/degrees/all")]
	public async Task<ActionResult<AvailableDataResponseDto<NameIdElementDto>>> GetAvailableStudyDegreeTypes()
	{
		return Ok(await helperService.GetAvailableStudyDegreeTypes());
	}

	[HttpGet("degrees/dept/{deptId:long}/all")]
	public async Task<ActionResult<List<NameIdElementDto>>> GetAvailableDegreesBaseStudySpec([FromRoute] long deptId)
	{
		return Ok(await helperService.GetAvailableStudyDegreeBaseAllSpecs(deptId));
	}

	[HttpGet("semester/dept/{deptId:long}/spec/{specId:long}/all")]
	public async Task<ActionResult<List<NameIdElementDto>>> GetAvailableSemBaseStudyGroup([FromRoute] long deptId,
		[FromRoute] long specId)
	{
		return Ok(await helperService.GetAvailableSemBaseStudyGroups(deptId, specId));
	}

	[HttpGet("semester/all")]
	public async Task<ActionResult<AvailableDataResponseDto<NameIdElementDto>>> GetAvailableSemesters()
	{
		return Ok(await helperService.GetAvailableSemesters());
	}

	[HttpGet("pagination/all")]
	public ActionResult<List<int>> GetAvailablePaginationSizes()
	{
		return Ok(helperService.GetAvailablePaginationTypes());
	}

	[HttpGet("room/type/all")]
	public async Task<ActionResult<AvailableDataResponseDto<string>>> GetAvailableRoomTypes()
	{
		return Ok(await helperService.GetAvailableRoomTypes());
	}

	[HttpGet("role/all")]
	public async Task<ActionResult<AvailableDataResponseDto<string>>> GetAvailableRoles()
	{
		return Ok(await helperService.GetAvailableRoles());
	}

	[HttpGet("subject/type/all")]
	public async Task<ActionResult<AvailableDataResponseDto<string>>> GetAvailableSubjectTypes(
		[FromQuery] string? subjectTypeName)
	{
		return Ok(await helperService.GetAvailableSubjectTypes(subjectTypeName));
	}

	[AuthorizeRoles(UserRole.Administrator, UserRole.Editor)]
	[HttpPost("schedule/name/to/tuple")]
	public async Task<ActionResult<ConvertToTupleResponseDto>> ConvertNamesToTuples(ConvertNamesToTuplesRequestDto dto)
	{
		return Ok(await helperService.ConvertNamesToTuples(dto));
	}
}
