using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Dto.AuthDtos;
using asp_net_po_schedule_management_server.CustomDecorators;


namespace asp_net_po_schedule_management_server.Controllers
{
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }
        
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> UserLogin([FromBody] LoginRequestDto user)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.UserLogin(user));
        }
        
        [HttpPost("register")]
        [AuthorizeRoles(AvailableRoles.ADMINISTRATOR)]
        public async Task<ActionResult<RegisterNewUserResponseDto>> UserRegister([FromBody] RegisterNewUserRequestDto user)
        {
            return StatusCode((int) HttpStatusCode.Created,  await _service.UserRegister(user));
        }
    }
}