using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Api.Attribute;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Network.User;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.Department;

[ApiController]
[Route("/api/v1/[controller]")]
[AuthorizeRoles(UserRole.Administrator)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DepartmentController(IDepartmentService departmentService) : ControllerBase
{
	[AllowAnonymous]
	[HttpGet("all")]
	public ActionResult<SearchQueryResponseDto> GetAllDepartments([FromQuery] string? name)
	{
		return Ok(departmentService.GetAllDepartments(name));
	}

	[HttpGet("all/pageable")]
	public ActionResult<UserResponseDto> GetPageableDepartments([FromQuery] SearchQueryRequestDto searchQuery)
	{
		return Ok(departmentService.GetPageableDepartments(searchQuery));
	}

	[AllowAnonymous]
	[HttpGet("all/schedule")]
	public async Task<ActionResult<List<NameIdElementDto>>> GetAllDepartmentsSchedule()
	{
		return Ok(await departmentService.GetAllDepartmentsSchedule());
	}

	[HttpGet("{deptId:long}/details")]
	public async Task<ActionResult<DepartmentEditResDto>> GetDepartmentDetails([FromRoute] long deptId)
	{
		return Ok(await departmentService.GetDepartmentDetails(deptId));
	}

	[HttpPost]
	public async Task<ActionResult<DepartmentRequestResponseDto>> CreateDepartment(
		[FromBody] DepartmentRequestResponseDto dto)
	{
		return Created(string.Empty, await departmentService.CreateDepartment(dto));
	}

	[HttpPut("{deptId:long}")]
	public async Task<ActionResult<DepartmentRequestResponseDto>> UpdateDepartment([FromRoute] long deptId,
		[FromBody] DepartmentRequestResponseDto dto)
	{
		return Ok(await departmentService.UpdateDepartment(dto, deptId));
	}

	[HttpDelete("selected")]
	public async Task<ActionResult<MessageContentResDto>> DeleteSelectedDepartments(
		[FromBody] DeleteSelectedRequestDto departments)
	{
		return Ok(await departmentService.Delete2WayFactorSelected(departments, HttpContext, Request));
	}

	[HttpDelete("all")]
	public async Task<ActionResult> DeleteAllDepartments()
	{
		return Ok(await departmentService.Delete2WayFactorAll(HttpContext, Request));
	}
}