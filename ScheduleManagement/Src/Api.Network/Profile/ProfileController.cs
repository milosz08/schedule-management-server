using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleManagement.Api.Dto;

namespace ScheduleManagement.Api.Network.Profile;

[ApiController]
[Route("/api/v1/[controller]/profile")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class FileController(IProfileService profileService) : ControllerBase
{
	[HttpPost]
	[DisableRequestSizeLimit]
	public async Task<ActionResult<MessageContentResDto>> CreateUserCustomAvatar([FromForm] IFormFile image)
	{
		return Ok(await profileService.CreateUserCustomAvatar(image, HttpContext.User));
	}

	[HttpDelete]
	public async Task<ActionResult<MessageContentResDto>> RemoveUserCustomAvatar()
	{
		return Ok(await profileService.RemoveUserCustomAvatar(HttpContext.User));
	}
}
