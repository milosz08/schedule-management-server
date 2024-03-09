using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Api.Attribute;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Network.User;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.StudySpec;

[ApiController]
[Route("/api/v1/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class StudySpecController(IStudySpecService studySpecService) : ControllerBase
{
	[AllowAnonymous]
	[HttpGet("schedule/dept/{deptId:long}/degree/{degreeId:long}/all")]
	public async Task<ActionResult<List<NameIdElementDto>>> GetAllStudySpecsScheduleBaseDept(
		[FromRoute] long deptId,
		[FromRoute] long degreeId)
	{
		return Ok(await studySpecService.GetAllStudySpecsScheduleBaseDept(deptId, degreeId));
	}

	[AllowAnonymous]
	[HttpGet("dept")]
	public async Task<ActionResult<AvailableDataResponseDto<NameIdElementDto>>> GetAvailableStudySpecsBaseDept(
		[FromQuery] string deptName)
	{
		return Ok(await studySpecService.GetAvailableStudySpecsBaseDept(deptName));
	}

	[AuthorizeRoles(UserRole.Administrator)]
	[HttpGet("all/pageable")]
	public ActionResult<PaginationResponseDto<StudySpecQueryResponseDto>> GetAllStudySpecializations(
		[FromQuery] SearchQueryRequestDto searchSearchQuery)
	{
		return Ok(studySpecService.GetAllStudySpecializations(searchSearchQuery));
	}

	[AuthorizeRoles(UserRole.Administrator, UserRole.Editor)]
	[HttpGet("all/dept")]
	public async Task<ActionResult<SearchQueryResponseDto>> GetAllStudySpecializationsBaseDept([FromQuery] string? spec,
		[FromQuery] string? dept)
	{
		return Ok(await studySpecService.GetAllStudySpecializationsInDepartment(spec, dept));
	}

	[AuthorizeRoles(UserRole.Administrator)]
	[HttpGet("{specId:long}/details")]
	public async Task<ActionResult<StudySpecializationEditResDto>> GetStudySpecializationDetails(
		[FromRoute] long specId)
	{
		return Ok(await studySpecService.GetStudySpecializationDetails(specId));
	}

	[AuthorizeRoles(UserRole.Administrator)]
	[HttpPost]
	public async Task<ActionResult<List<StudySpecResponseDto>>> CreateStudySpecialization(
		[FromBody] StudySpecRequestDto dto)
	{
		return Created(string.Empty, await studySpecService.CreateStudySpecialization(dto));
	}

	[HttpPut("{specId:long}")]
	public async Task<ActionResult<List<StudySpecResponseDto>>> UpdateStudySpecialization(
		[FromBody] StudySpecRequestDto dto,
		[FromRoute] long specId)
	{
		return Ok(await studySpecService.UpdateStudySpecialization(dto, specId));
	}

	[AuthorizeRoles(UserRole.Administrator)]
	[HttpDelete("selected")]
	public async Task<ActionResult<MessageContentResDto>> DeleteMassiveStudySpecs(
		[FromBody] DeleteSelectedRequestDto specs)
	{
		return Ok(await studySpecService.Delete2WayFactorSelected(specs, HttpContext, Request));
	}

	[AuthorizeRoles(UserRole.Administrator)]
	[HttpDelete("all")]
	public async Task<ActionResult<MessageContentResDto>> DeleteAllStudySpecs()
	{
		return Ok(await studySpecService.Delete2WayFactorAll(HttpContext, Request));
	}
}
