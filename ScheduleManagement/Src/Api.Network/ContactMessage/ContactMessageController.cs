using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Api.Db;
using ScheduleManagement.Api.Dto;
using ScheduleManagement.Api.Pagination;

namespace ScheduleManagement.Api.Network.ContactMessage;

[ApiController]
[Route("/api/v1/[controller]")]
public sealed class ContactMessageController(IContactMessageService contactMessageService)
	: ControllerBase
{
	[HttpGet("issue/type/all")]
	public async Task<ActionResult<AvailableDataResponseDto<string>>> GetAllContactMessageIssueTypes(
		[FromQuery] string? issueTypeName)
	{
		return Ok(await contactMessageService.GetAllContactMessageIssueTypes(issueTypeName));
	}

	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[HttpGet("user/all")]
	public async Task<ActionResult<PaginationResponseDto<ContactMessagesQueryResponseDto>>> GetAllMessagesBaseUser(
		[FromQuery] SearchQueryRequestDto searchQuery)
	{
		return StatusCode((int)HttpStatusCode.OK, await contactMessageService
			.GetAllMessagesBaseClaims(searchQuery, HttpContext.User));
	}

	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[HttpGet("user/details/{messId:long}")]
	public async Task<ActionResult<SingleContactMessageResponseDto>> GetContactMessageDetails([FromRoute] long messId)
	{
		return Ok(await contactMessageService.GetContactMessageDetails(messId, HttpContext.User));
	}

	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[HttpPost("new")]
	public async Task<ActionResult<MessageContentResDto>> CreateMessage([FromBody] ContactMessagesReqDto dto)
	{
		return Created(string.Empty, await contactMessageService.CreateMessage(dto, HttpContext.User));
	}

	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[HttpDelete("selected")]
	public async Task<ActionResult<MessageContentResDto>> DeleteSelectedContactMessages(
		[FromBody] DeleteSelectedRequestDto dto)
	{
		return Ok(await contactMessageService.Delete2WayFactorSelected(dto, HttpContext, Request));
	}

	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[HttpDelete("all")]
	public async Task<ActionResult<MessageContentResDto>> DeleteAllContactMessages()
	{
		return Ok(await contactMessageService.Delete2WayFactorAll(HttpContext, Request));
	}
}
