using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.CustomDecorators;
using asp_net_po_schedule_management_server.Exceptions;

using asp_net_po_schedule_management_server.Dto.Misc;
using asp_net_po_schedule_management_server.Dto.Requests;
using asp_net_po_schedule_management_server.Dto.Responses;


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
        public ActionResult<UserResponseDto> GetAllUsers([FromQuery] UserQueryRequestDto searchQuery)
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAllUsers(searchQuery));
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [HttpDelete(ApiEndpoints.DELETE_MASSIVE_USERS)]
        public async Task<ActionResult> DeleteMassiveUsers([FromBody] MassiveDeleteRequestDto deleteUsers)
        {
            await _service.DeleteMassiveUsers(deleteUsers, ExtractedUserCredentialsFromHeader());
            return StatusCode((int) HttpStatusCode.NoContent);
        }

        //--------------------------------------------------------------------------------------------------------------

        [HttpDelete(ApiEndpoints.DELETE_ALL_USERS)]
        public async Task<ActionResult> DeleteAllUsers()
        {
            await _service.DeleteAllUsers(ExtractedUserCredentialsFromHeader());
            return StatusCode((int) HttpStatusCode.NoContent);
        }

        //--------------------------------------------------------------------------------------------------------------
        
        #region Helper methods
        
        /// <summary>
        /// Metoda eksportująca nagłówki autoryzacji i wartość claim reprezentującą login użytkownika (zaszytą w
        /// JWT) i opakowująca wszystkie dane w obiekt.
        /// </summary>
        /// <returns>Obiekt z danymi autoryzacji: login, nazwa użytkownika i hasło</returns>
        /// <exception cref="BasicServerException">Brak nagłówków autoryzacji</exception>
        private UserCredentialsHeaderDto ExtractedUserCredentialsFromHeader()
        {
            Claim userLogin = HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Name);
            IHeaderDictionary headers = this.Request.Headers;
            if (headers.TryGetValue("User-Name", out var username) 
                && headers.TryGetValue("User-Password", out var password)) {
                return new UserCredentialsHeaderDto()
                {
                    Login = userLogin?.Value,
                    Username = username.First(),
                    Password = password.First(),
                };
            }
            throw new BasicServerException("Brak nagłówków autoryzacji!", HttpStatusCode.Forbidden);
        }

        #endregion
    }
}