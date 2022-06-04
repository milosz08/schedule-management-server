using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        [HttpGet(ApiEndpoints.GET_ALL_USERS_PAGINATION)]
        public ActionResult<UserResponseDto> GetAllUsers([FromQuery] SearchQueryRequestDto searchQuery)
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAllUsers(searchQuery));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [AllowAnonymous]
        [HttpGet(ApiEndpoints.GET_ALL_EMPLOYEERS_SCHEDULE)]
        public async Task<ActionResult<List<NameWithDbIdElement>>> GetAllEmployeersScheduleBaseCath(
            [FromQuery] long deptId,
            [FromQuery] long cathId)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAllEmployeersScheduleBaseCath(deptId, cathId));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [AllowAnonymous]
        [HttpGet(ApiEndpoints.GET_ALL_TEACHERS_BASE_DEPT_AND_SUBJ)]
        public async Task<ActionResult<List<NameWithDbIdElement>>> GetAllTeachersScheduleBaseDeptAndSpec(
            [FromQuery] long deptId,
            [FromQuery] string subjName)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service
                .GetAllTeachersScheduleBaseDeptAndSpec(deptId, subjName));
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [AllowAnonymous]
        [HttpGet(ApiEndpoints.GET_DASHBOARD_DETAILS)]
        public async Task<ActionResult<DashboardDetailsResDto>> GetDashboardPanelData()
        {
            Claim userIdentity = HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Name);   
            return StatusCode((int) HttpStatusCode.OK, await _service.GetDashboardPanelData(userIdentity));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpDelete(ApiEndpoints.DELETE_MASSIVE)]
        public async Task<ActionResult> DeleteMassiveUsers([FromBody] MassiveDeleteRequestDto deleteUsers)
        {
            await _service.DeleteMassiveUsers(deleteUsers, _helper
                .ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }

        //--------------------------------------------------------------------------------------------------------------

        [HttpDelete(ApiEndpoints.DELETE_ALL)]
        public async Task<ActionResult> DeleteAllUsers()
        {
            await _service.DeleteAllUsers(_helper.ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }
    }
}