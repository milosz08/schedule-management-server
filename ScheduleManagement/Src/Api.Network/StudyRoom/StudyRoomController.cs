using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Api.Attribute;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Network.User;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.StudyRoom;

[ApiController]
[Route("/api/v1/[controller]")]
[AuthorizeRoles(UserRole.Administrator)]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class StudyRoomController(IStudyRoomService studyRoomService) : ControllerBase
{
	[HttpGet("all")]
	public ActionResult<StudyRoomQueryResponseDto> GetStudyRooms([FromQuery] SearchQueryRequestDto searchQuery)
	{
		return Ok(studyRoomService.GetAllStudyRooms(searchQuery));
	}

	[HttpGet("{roomId:long}/details")]
	public async Task<ActionResult<StudyRoomEditResDto>> GetStudyRoomDetails([FromRoute] long roomId)
	{
		return Ok(await studyRoomService.GetStudyRoomDetails(roomId));
	}

	[AllowAnonymous]
	[HttpGet("dept/{deptId:long}")]
	public async Task<ActionResult<List<NameIdElementDto>>> GetAllStudyRoomsScheduleBaseDeptName(
		[FromRoute] long deptId)
	{
		return Ok(await studyRoomService.GetAllStudyRoomsScheduleBaseDeptName(deptId));
	}

	[AllowAnonymous]
	[HttpGet("dept/{deptId:long}/cath:/{cathId:long}")]
	public async Task<ActionResult<List<NameIdElementDto>>> GetAllStudyRoomsScheduleBaseCath(
		[FromRoute] long deptId,
		[FromRoute] long cathId)
	{
		return Ok(await studyRoomService.GetAllStudyRoomsScheduleBaseCath(deptId, cathId));
	}

	[HttpPost]
	public async Task<ActionResult<StudyRoomResponseDto>> CreateStudyRoom(StudyRoomRequestDto dto)
	{
		return Created(string.Empty, await studyRoomService.CreateStudyRoom(dto));
	}

	[HttpPut("room/{roomId:long}")]
	public async Task<ActionResult<StudyRoomResponseDto>> UpdateStudyRoom(
		[FromBody] StudyRoomRequestDto dto,
		[FromRoute] long roomId)
	{
		return Ok(await studyRoomService.UpdateStudyRoom(dto, roomId));
	}

	[HttpDelete("selected")]
	public async Task<ActionResult<MessageContentResDto>> DeleteSelectedStudyRooms(
		[FromBody] DeleteSelectedRequestDto rooms)
	{
		return Ok(await studyRoomService.Delete2WayFactorSelected(rooms, HttpContext, Request));
	}

	[HttpDelete("all")]
	public async Task<ActionResult<MessageContentResDto>> DeleteAllStudyRooms()
	{
		return Ok(await studyRoomService.Delete2WayFactorAll(HttpContext, Request));
	}
}
