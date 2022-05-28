using System.Net;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.CustomDecorators;


namespace asp_net_po_schedule_management_server.Controllers
{
    /// <summary>
    /// Kontroler przechowujący metody odpowiadające za endpointy związane z czasem, datami i godzinami.
    /// </summary>
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    [AuthorizeRoles(AvailableRoles.EDITOR, AvailableRoles.ADMINISTRATOR)]
    public sealed class TimeManagementController : ControllerBase
    {
        private readonly ITimeManagementService _service;
       
        //--------------------------------------------------------------------------------------------------------------
        
        public TimeManagementController(ITimeManagementService service)
        {
            _service = service;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_WEEKSDATA_BASE_CURR_YEAR)]
        public ActionResult<List<string>> GetAllWeeksNameWithWeekNumberInCurrentYear()
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAllWeeksNameWithWeekNumberInCurrentYear());
        }
    }
}