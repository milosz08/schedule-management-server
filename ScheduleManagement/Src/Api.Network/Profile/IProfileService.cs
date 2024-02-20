using System.Security.Claims;
using ScheduleManagement.Api.Dto;

namespace ScheduleManagement.Api.Network.Profile;

public interface IProfileService
{
	Task<(byte[], string)> GetUserCustomAvatar(ClaimsPrincipal claimsPrincipal);

	Task<MessageContentResDto> CreateUserCustomAvatar(IFormFile image, ClaimsPrincipal claimsPrincipal);

	Task<MessageContentResDto> RemoveUserCustomAvatar(ClaimsPrincipal claimsPrincipal);
}
