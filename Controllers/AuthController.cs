using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.CustomDecorators;

using asp_net_po_schedule_management_server.Dto.AuthDtos;
using asp_net_po_schedule_management_server.Dto.Requests;
using asp_net_po_schedule_management_server.Dto.Responses;
using asp_net_po_schedule_management_server.Dto.CrossQuery;


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
        [HttpPost(ApiEndpoints.LOGIN)]
        public async Task<ActionResult<LoginResponseDto>> UserLogin([FromBody] LoginRequestDto user)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.UserLogin(user));
        }
        
        [HttpPost(ApiEndpoints.REGISTER)]
        [AuthorizeRoles(AvailableRoles.ADMINISTRATOR)]
        public async Task<ActionResult<RegisterNewUserResponseDto>> UserRegister([FromBody] RegisterNewUserRequestDto user)
        {
            return StatusCode((int) HttpStatusCode.Created, await _service.UserRegister(user));
        }
        
        [HttpPost(ApiEndpoints.CHANGE_PASSWORD)]
        public async Task<ActionResult<PseudoNoContentResponseDto>> UserFirstChangePassword(
            [FromQuery] string userId,
            [FromBody] ChangePasswordRequestDto form)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.UserChangePassword(form, userId));
        }

        [AllowAnonymous]
        [HttpPost(ApiEndpoints.REFRESH_TOKEN)]
        public async Task<ActionResult<RefreshTokenDto>> UserRefreshToken(RefreshTokenDto dto)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.UserRefreshToken(dto));
        }
    }
}