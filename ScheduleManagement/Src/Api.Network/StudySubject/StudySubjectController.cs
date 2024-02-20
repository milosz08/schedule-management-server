using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Api.Attribute;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Network.User;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.StudySubject;

[ApiController]
[Route("/api/v1/[controller]")]
[AuthorizeRoles(UserRole.Administrator)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class StudySubjectController(IStudySubjectService studySubjectService) : ControllerBase
{
	[AllowAnonymous]
	[HttpGet("all")]
	public ActionResult<StudySubjectQueryResponseDto> GetStudyRooms([FromQuery] SearchQueryRequestDto searchQuery)
	{
		return Ok(studySubjectService.GetAllStudySubjects(searchQuery));
	}

	[AllowAnonymous]
	[HttpGet("dept/{deptId:long}/spec/{specId:long}/all")]
	public ActionResult<SearchQueryResponseDto> GetAllStudySubjectsBaseDept([FromQuery] string? subjectName,
		[FromRoute] long deptId,
		[FromRoute] long specId)
	{
		return Ok(studySubjectService.GetAllStudySubjectsBaseDeptAndSpec(subjectName, deptId, specId));
	}

	[AllowAnonymous]
	[HttpGet("dept/all")]
	public async Task<ActionResult<AvailableDataResponseDto<NameIdElementDto>>> GetAvailableStudySubjectsBaseDept(
		[FromQuery] string deptName)
	{
		return Ok(await studySubjectService.GetAvailableSubjectsBaseDept(deptName));
	}

	[HttpGet("{subjectId:long}/details")]
	public async Task<ActionResult<StudySubjectEditResDto>> GetStudySubjectBaseDbId([FromRoute] long subjectId)
	{
		return Ok(await studySubjectService.GetStudySubjectBaseDbId(subjectId));
	}

	[HttpPost]
	public async Task<ActionResult<StudySubjectResponseDto>> CreateStudySubject([FromBody] StudySubjectRequestDto dto)
	{
		return Created(string.Empty, await studySubjectService.CreateStudySubject(dto));
	}

	[HttpPut("{subjectId:long}")]
	public async Task<ActionResult<StudySubjectResponseDto>> UpdateStudySubject([FromBody] StudySubjectRequestDto dto,
		[FromRoute] long subjectId)
	{
		return Ok(await studySubjectService.UpdateStudySubject(dto, subjectId));
	}

	[HttpDelete("selected")]
	public async Task<ActionResult<MessageContentResDto>> DeleteMassiveSubjects(
		[FromBody] DeleteSelectedRequestDto subjects)
	{
		return Ok(await studySubjectService.Delete2WayFactorSelected(subjects, HttpContext, Request));
	}

	[HttpDelete("all")]
	public async Task<ActionResult<MessageContentResDto>> DeleteAllSubjects()
	{
		return Ok(await studySubjectService.Delete2WayFactorAll(HttpContext, Request));
	}
}