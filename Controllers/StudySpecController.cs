using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

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
    public sealed class StudySpecController : ControllerBase
    {
        private IStudySpecService _service;
        
        //--------------------------------------------------------------------------------------------------------------

        public StudySpecController(IStudySpecService service)
        {
            _service = service;
        }

        //--------------------------------------------------------------------------------------------------------------

        [HttpPost(ApiEndpoints.ADD_STUDY_SPECIALIZATION)]
        public async Task<ActionResult<IEnumerable<StudySpecResponseDto>>> AddNewStudySpecialization(StudySpecRequestDto dto)
        {
            return StatusCode((int) HttpStatusCode.Created, await _service.AddNewStudySpecialization(dto));
        }
    }
}