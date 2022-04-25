using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;

using asp_net_po_schedule_management_server.Dto.Responses;


namespace asp_net_po_schedule_management_server.Controllers
{
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public sealed class FileController : ControllerBase
    {
        private readonly IFilesService _service;

        public FileController(IFilesService service)
        {
            _service = service;
        }
        
        [HttpGet(ApiEndpoints.GET_AVATAR)]
        public async Task<ActionResult> UserGetCustomAvatar([FromQuery] string userId)
        {
            Claim userLogin = HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Name);
            (byte[], string) imageTuple = await _service.UserGetCustomAvatar(userId, userLogin);
            return File(imageTuple.Item1, imageTuple.Item2);
        }
        
        [HttpPost(ApiEndpoints.ADD_AVATAR)]
        public async Task<ActionResult<PseudoNoContentResponseDto>> UserAddCustomAvatar([FromForm] IFormFile image)
        {
            Claim userLogin = HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Name);
            return StatusCode((int) HttpStatusCode.OK, await _service.UserAddCustomAvatar(image, userLogin));
        }
    }
}