using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Services.Helpers;
using asp_net_po_schedule_management_server.CustomDecorators;


namespace asp_net_po_schedule_management_server.Controllers
{
    /// <summary>
    /// Kontroler przechowujący akcje do zarządzania encją użytkowników w systemie. Umożliwia pobranie wszystkich
    /// użytkowników systemu oraz zaawanowaną filtrację wyników i paginację, pobieranie użytkowników na podstawie
    /// wybranej roli i dodatkowe filtrowanie (po nazwie) oraz standardowe metody usuwające zawartość z bazy danych.
    /// </summary>
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    [AuthorizeRoles(AvailableRoles.ADMINISTRATOR)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public sealed class UsersController : ControllerBase
    {
        private readonly ServiceHelper _helper;
        private readonly IUsersService _service;

        //--------------------------------------------------------------------------------------------------------------
        
        public UsersController(ServiceHelper helper, IUsersService service)
        {
            _helper = helper;
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
    }
}