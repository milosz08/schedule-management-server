using System.Net;

using Microsoft.AspNetCore.Mvc;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Dto.Responses;


namespace asp_net_po_schedule_management_server.Controllers
{
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

        [HttpGet(ApiEndpoints.GET_AVAILABLE_STUDY_TYPES)]
        public ActionResult<AvailableStudyTypesResponseDto> GetAvailableStudyTypes()
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAvailableStudyTypes());
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [HttpGet(ApiEndpoints.GET_AVAILABLE_ROOM_TYPES)]
        public ActionResult<AvailableRoomTypesResponseDto> GetAvailableRoomTypes()
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAvailableRoomTypes());
        }
    }
}