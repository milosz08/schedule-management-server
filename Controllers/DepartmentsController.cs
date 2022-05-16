using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.CustomDecorators;

using asp_net_po_schedule_management_server.Dto.Responses;
using asp_net_po_schedule_management_server.Dto.RequestResponseMerged;


namespace asp_net_po_schedule_management_server.Controllers
{
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    [AuthorizeRoles(AvailableRoles.ADMINISTRATOR)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public sealed class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentsService _service;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public DepartmentsController(IDepartmentsService service)
        {
            _service = service;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpPost(ApiEndpoints.ADD_DEPARTMENT)]
        public async Task<ActionResult<CreateDepartmentRequestResponseDto>> CreateDepartment(
            [FromBody] CreateDepartmentRequestResponseDto dto)
        {
            return StatusCode((int) HttpStatusCode.Created, await _service.CreateDepartment(dto));
        }

        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_ALL_DEPARTMENTS_LIST)]
        public ActionResult<SearchQueryResponseDto> GetAllDepartmentsList([FromQuery] string deptQuerySearch)
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAllDepartmentsList(deptQuerySearch));
        }
    }
}