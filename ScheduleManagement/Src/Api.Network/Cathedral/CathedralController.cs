using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Api.Attribute;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Network.User;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.Cathedral;

[ApiController]
[Route("/api/v1/[controller]")]
[AuthorizeRoles(UserRole.Administrator)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class CathedralController(ICathedralService cathedralService) : ControllerBase
{
	[HttpGet("all/pageable")]
	public ActionResult<UserResponseDto> GetCathedrals([FromQuery] SearchQueryRequestDto searchQuery)
	{
		return Ok(cathedralService.GetAllCathedrals(searchQuery));
	}

	[HttpGet("dept/all")]
	public async Task<ActionResult<SearchQueryResponseDto>> GetAllCathedralsBasedDepartmentName(
		[FromQuery] string? deptName,
		[FromQuery] string? cathName
	)
	{
		return Ok(await cathedralService.GetAllCathedralsBasedDepartmentName(deptName, cathName));
	}

	[AllowAnonymous]
	[HttpGet("schedule/department/{deptId:long}")]
	public async Task<ActionResult<List<NameIdElementDto>>> GetAllCathedralsScheduleBaseDept([FromRoute] long deptId)
	{
		return Ok(await cathedralService.GetAllCathedralsScheduleBaseDept(deptId));
	}

	[HttpGet("{cathId:long}/details")]
	public async Task<ActionResult<CathedralEditResDto>> GetCathedralDetails([FromRoute] long cathId)
	{
		return Ok(await cathedralService.GetCathedralDetails(cathId));
	}

	[HttpPost]
	public async Task<ActionResult<CathedralResponseDto>> CreateCathedral([FromBody] CathedralRequestDto dto)
	{
		return Created(string.Empty, await cathedralService.CreateCathedral(dto));
	}

	[HttpPut("{cathId:long}")]
	public async Task<ActionResult<CathedralResponseDto>> UpdateCathedral([FromRoute] long cathId,
		[FromBody] CathedralRequestDto dto)
	{
		return Ok(await cathedralService.UpdateCathedral(dto, cathId));
	}

	[HttpDelete("selected")]
	public async Task<ActionResult<MessageContentResDto>> DeleteSelectedCathedrals(
		[FromBody] DeleteSelectedRequestDto cathedrals)
	{
		return Ok(await cathedralService.Delete2WayFactorSelected(cathedrals, HttpContext, Request));
	}

	[HttpDelete("all")]
	public async Task<ActionResult<MessageContentResDto>> DeleteAllCathedrals()
	{
		return Ok(await cathedralService.Delete2WayFactorAll(HttpContext, Request));
	}
}
