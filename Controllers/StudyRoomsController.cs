using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.CustomDecorators;
using asp_net_po_schedule_management_server.Dto.RequestResponseMerged;


namespace asp_net_po_schedule_management_server.Controllers
{
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    [AuthorizeRoles(AvailableRoles.ADMINISTRATOR)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public sealed class StudyRoomsController : ControllerBase
    {
        private readonly IStudyRoomsService _service;

        //--------------------------------------------------------------------------------------------------------------
        
        public StudyRoomsController(IStudyRoomsService service)
        {
            _service = service;
        }

        //--------------------------------------------------------------------------------------------------------------
        
        [HttpPost(ApiEndpoints.ADD_STUDY_ROOM)]
        public async Task<ActionResult<CreateStudyRoomResponseDto>> AddNewStudyRoom(CreateStudyRoomRequestDto dto)
        {
            return StatusCode((int) HttpStatusCode.Created, await _service.CreateStudyRoom(dto));
        }
    }
}