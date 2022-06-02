using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;


namespace asp_net_po_schedule_management_server.Controllers
{
    /// <summary>
    /// Kontroler odpowiadający za masowe zapytania do API ze strony wyszukiwania treści. Kontroler niechroniony.
    /// </summary>
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    public sealed class SearchContentController : ControllerBase
    {
        private readonly ISearchContentService _service;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public SearchContentController(ISearchContentService service)
        {
            _service = service;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_MASSIVE_QUERY_RESULT)]
        public async Task<ActionResult<List<SearchMassiveQueryResDto>>> GetAllItemsFromMassiveServerQuery(
            [FromQuery] SearchMassiveQueryReqDto dto)
        { 
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAllItemsFromMassiveServerQuery(dto));
        }
    }
}