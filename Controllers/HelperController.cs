using System.Net;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

using asp_net_po_schedule_management_server.Dto.Responses;
using asp_net_po_schedule_management_server.Utils;


namespace asp_net_po_schedule_management_server.Controllers
{
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    public sealed class HelperController : ControllerBase
    {
        [HttpGet(ApiEndpoints.GET_AVAILABLE_PAGINATIONS)]
        public ActionResult<AvailablePaginationSizes> GetAvailablePaginationSizes()
        {
            return StatusCode((int) HttpStatusCode.OK, new AvailablePaginationSizes()
            {
                AvailablePaginations = ApplicationUtils._allowedPageSizes.ToList(),
            });
        }
    }
}