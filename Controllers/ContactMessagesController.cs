using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Services.Helpers;


namespace asp_net_po_schedule_management_server.Controllers
{
    /// <summary>
    /// Kontroler obsługujący rządzania wiadomości użytkowników. Umożliwia dodanie wiadomości, pobranie wiadomości z
    /// zastosowaniem sortowania, pobranie szczegółów pojedynczej wiadomości oraz usuwanie wybranych wiadomości. Część
    /// z endpointów chroniona jest poprzez JWT i w zależności od roli użytkownika, metody zwracają odpowiednie dane.
    /// </summary>
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    public sealed class ContactMessagesController : ControllerBase
    {
        private readonly ServiceHelper _helper;
        private readonly IContactMessagesService _service;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public ContactMessagesController(IContactMessagesService service, ServiceHelper helper)
        {
            _service = service;
            _helper = helper;
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [HttpPost(ApiEndpoints.ADD_NEW_CONTACT_MESSAGE)]
        public async Task<ActionResult<PseudoNoContentResponseDto>> AddNewMessage([FromBody] ContactMessagesReqDto dto)
        {
            Claim userIdentity = HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Name);
            return StatusCode((int) HttpStatusCode.Created, await _service.AddNewMessage(dto, userIdentity));
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [HttpGet(ApiEndpoints.GET_ALL_CONTACT_MESSAGE_ISSUE_TYPES)]
        public async Task<ActionResult<AvailableDataResponseDto<string>>> AllContactMessageIssueTypes(
            [FromQuery] string issueTypeName)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAllContactMessageIssueTypes(issueTypeName));
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(ApiEndpoints.GET_ALL_MESSAGES_BASE_CLAIMS)]
        public async Task<ActionResult<PaginationResponseDto<ContactMessagesQueryResponseDto>>> AllMessagesBaseUserClaims(
            [FromQuery] SearchQueryRequestDto searchQuery)
        {
            Claim userRole = HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Role);
            Claim userLogin = HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Name);
            return StatusCode((int) HttpStatusCode.OK, await _service
                .GetAllMessagesBaseClaims(searchQuery, userRole, userLogin));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(ApiEndpoints.GET_MESSAGE_BASE_ID_AND_CLAIMS)]
        public async Task<ActionResult<SingleContactMessageResponseDto>> GetContactMessageBaseId(
            [FromQuery] long messId)
        {
            Claim userRole = HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Role);
            Claim userLogin = HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Name);
            return StatusCode((int) HttpStatusCode.OK, await _service.GetContactMessageBaseId(messId, userRole, userLogin));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete(ApiEndpoints.DELETE_MASSIVE)]
        public async Task<ActionResult> DeleteMassiveContactMessages([FromBody] MassiveDeleteRequestDto dto)
        {
            await _service.DeleteMassiveContactMess(dto, await _helper
                .ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete(ApiEndpoints.DELETE_ALL)]
        public async Task<ActionResult> DeleteAllContactMessages()
        {
            await _service.DeleteAllContactMess(await _helper
                .ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }
    }
}