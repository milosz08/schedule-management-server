using Microsoft.AspNetCore.Mvc;

namespace ScheduleManagement.Api.Network.MemoryStorage;

[ApiController]
[Route("/api/v1/[controller]")]
public class MemoryStorageController(IMemoryStorageService memoryStorageService) : ControllerBase
{
	[HttpPatch("accounts/check")]
	public async Task<ActionResult<List<SavedAccountDetailsResponseDto>>> CheckSavedAccounts(
		[FromBody] SavedAccountsRequestDto reqDto)
	{
		return Ok(await memoryStorageService.CheckSavedAccounts(reqDto));
	}
}
