using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using asp_net_po_schedule_management_server.Services;

namespace asp_net_po_schedule_management_server.Controllers
{
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    public sealed class FirstEndpointController : ControllerBase
    {
        private readonly ILogger<FirstEndpointController> _logger;
        private readonly IFirstEndpointService _service;

        public FirstEndpointController(ILogger<FirstEndpointController> logger, IFirstEndpointService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("getinfo/{first:int}/{second:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public ActionResult<string> GetMessageValue([FromRoute] int first, [FromRoute] int second)
        {
            return StatusCode((int)HttpStatusCode.OK, _service.AdditionService(first, second));
        }
    }
}