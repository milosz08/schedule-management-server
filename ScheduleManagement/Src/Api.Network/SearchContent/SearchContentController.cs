using Microsoft.AspNetCore.Mvc;

namespace ScheduleManagement.Api.Network.SearchContent;

[ApiController]
[Route("/api/v1/[controller]")]
public class SearchContentController(ISearchContentService searchContentService) : ControllerBase
{
	[HttpGet]
	public async Task<ActionResult<List<SearchMassiveQueryResDto>>> GetAllItemsFromServerQuery(
		[FromQuery] SearchMassiveQueryReqDto dto)
	{
		return Ok(await searchContentService.GetAllItemsFromServerQuery(dto));
	}
}
