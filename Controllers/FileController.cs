using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;


namespace asp_net_po_schedule_management_server.Controllers
{
    /// <summary>
    /// Kontroler przechowujący akcje do zarządzania plikami w systemie. Obsługuje przede wszystkim dodawanie oraz
    /// usuwanie zdjęcia użytkownika.
    /// </summary>
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public sealed class FileController : ControllerBase
    {
        private readonly IFilesService _service;

        //--------------------------------------------------------------------------------------------------------------
        
        public FileController(IFilesService service)
        {
            _service = service;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_AVATAR)]
        public async Task<ActionResult> UserGetCustomAvatar([FromQuery] string userId)
        {
            Claim userLogin = HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Name);
            (byte[], string) imageTuple = await _service.UserGetCustomAvatar(userId, userLogin);
            return File(imageTuple.Item1, imageTuple.Item2);
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpPost(ApiEndpoints.ADD_OR_CHANGE_AVATAR), DisableRequestSizeLimit]
        public async Task<ActionResult<PseudoNoContentResponseDto>> UserAddCustomAvatar([FromForm] IFormFile image)
        {
            Claim userLogin = HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Name);
            return StatusCode((int) HttpStatusCode.OK, await _service.UserAddCustomAvatar(image, userLogin));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpDelete(ApiEndpoints.DELETE_AVATAR)]
        public async Task<ActionResult<PseudoNoContentResponseDto>> UserRemoveCustomAvatar()
        {
            Claim userLogin = HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Name);
            return StatusCode((int) HttpStatusCode.OK, await _service.UserRemoveCustomAvatar(userLogin));
        }
    }
}