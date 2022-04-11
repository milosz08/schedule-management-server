using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Services;


namespace asp_net_po_schedule_management_server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public ActionResult<UserResponseDto> Post([FromBody] UserRequestDto user)
        {
            return StatusCode((int) HttpStatusCode.OK, _service.AuthenticateUser(user));
        }
    }
}