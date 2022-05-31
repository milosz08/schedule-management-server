using System.Net;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;


namespace asp_net_po_schedule_management_server.Controllers
{
    /// <summary>
    /// Kontroler przechowujący metody odpowiadające za endpointy związane z czasem, datami i godzinami.
    /// </summary>
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    public sealed class TimeManagementController : ControllerBase
    {
        private readonly ITimeManagementService _service;
       
        //--------------------------------------------------------------------------------------------------------------
        
        public TimeManagementController(ITimeManagementService service)
        {
            _service = service;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_STUDY_YEARS)]
        public ActionResult<List<string>> GetAllStudyYearsFrom2020ToCurrent()
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAllStudyYearsFrom2020ToCurrent());
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_WEEKSDATA_BASE_CURR_YEAR)]
        public ActionResult<List<string>> GetAllWeeksNameWithWeekNumberInCurrentYear(
            [FromQuery] int startYear, [FromQuery] int endYear)
        {
            return StatusCode((int) HttpStatusCode.OK, _service
                .GetAllWeeksNameWithWeekNumberInCurrentYear(startYear, endYear));
        }
    }
}