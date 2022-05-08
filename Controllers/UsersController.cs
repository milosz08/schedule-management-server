using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.CustomDecorators;
using asp_net_po_schedule_management_server.Dto.Requests;


namespace asp_net_po_schedule_management_server.Controllers
{
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    [AuthorizeRoles(AvailableRoles.ADMINISTRATOR)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _service;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public UsersController(IUsersService service)
        {
            _service = service;
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [HttpGet(ApiEndpoints.GET_ALL_USERS)]
        public ActionResult GetAllUsers([FromQuery] UserQueryRequestDto searchQuery)
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAllUsers(searchQuery));
        }
    }
}