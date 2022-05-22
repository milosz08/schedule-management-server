using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;


namespace asp_net_po_schedule_management_server.Controllers
{
    /// <summary>
    /// Kontroler niechroniony (endpointy dostępne bez użycia JWT) przechowujący metody służące głównie do pobierania
    /// danych z bazy danych (np. w select boxach czy combo boxach na front-endzie).
    /// </summary>
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    public sealed class HelperController : ControllerBase
    {
        private readonly IHelperService _service;

        //--------------------------------------------------------------------------------------------------------------
        
        public HelperController(IHelperService service)
        {
            _service = service;
        }

        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_AVAILABLE_PAGINATIONS)]
        public ActionResult<AvailablePaginationSizes> GetAvailablePaginationSizes()
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAvailablePaginationTypes());
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [HttpGet(ApiEndpoints.GET_AVAILABLE_ROOM_TYPES)]
        public async Task<ActionResult<AvailableDataResponseDto<string>>> GetAvailableRoomTypes()
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAvailableRoomTypes());
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [HttpGet(ApiEndpoints.GET_AVAILABLE_ROLES)]
        public async Task<ActionResult<AvailableDataResponseDto<string>>> GetAvailableRoles()
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAvailableRoles());
        }
    }
}