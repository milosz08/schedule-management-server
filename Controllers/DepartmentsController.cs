using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Services.Helpers;
using asp_net_po_schedule_management_server.CustomDecorators;


namespace asp_net_po_schedule_management_server.Controllers
{
    /// <summary>
    /// Kontroler przechowujący akcje do zarządzania encją wydziałów w systemie. Umożliwia stworzenie wydziału, pobranie
    /// wszystkich wyników oraz zaawanowaną filtrację wyników i paginację, pobieranie wydziałów i dodatkowe filtrowanie
    /// oraz standardowe metody usuwające zawartość z bazy danych.
    /// </summary>
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    [AuthorizeRoles(AvailableRoles.ADMINISTRATOR)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public sealed class DepartmentsController : ControllerBase
    {
        private readonly ServiceHelper _helper;
        private readonly IDepartmentsService _service;

        //--------------------------------------------------------------------------------------------------------------
        
        public DepartmentsController(IDepartmentsService service, ServiceHelper helper)
        {
            _service = service;
            _helper = helper;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpPost(ApiEndpoints.ADD_DEPARTMENT)]
        public async Task<ActionResult<DepartmentRequestResponseDto>> CreateDepartment(
            [FromBody] DepartmentRequestResponseDto dto)
        {
            return StatusCode((int) HttpStatusCode.Created, await _service.CreateDepartment(dto));
        }

        //--------------------------------------------------------------------------------------------------------------
        
        [AllowAnonymous]
        [HttpGet(ApiEndpoints.GET_ALL_DEPARTMENTS_LIST)]
        public ActionResult<SearchQueryResponseDto> GetAllDepartmentsList([FromQuery] string deptQuerySearch)
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAllDepartmentsList(deptQuerySearch));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_ALL_DEPARTMENTS_PAGINATION)]
        public ActionResult<UserResponseDto> GetDepartments([FromQuery] SearchQueryRequestDto searchSearchQuery)
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAllDepartments(searchSearchQuery));
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [AllowAnonymous]
        [HttpGet(ApiEndpoints.GET_ALL_DEPARTMENTS_SCHEDULE)]
        public async Task<ActionResult<List<NameWithDbIdElement>>> GetAllDepartmentsSchedule()
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAllDepartmentsSchedule());
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_DEPARTMENT_BASE_ID)]
        public async Task<ActionResult<DepartmentEditResDto>> GetDepartmentBaseDbId([FromQuery] long deptId)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetDepartmentBaseDbId(deptId));
        }

        //--------------------------------------------------------------------------------------------------------------
        
        [HttpPut(ApiEndpoints.UPDATE_DEPARTMENT)]
        public async Task<ActionResult<DepartmentRequestResponseDto>> UpdateDepartment(
            [FromBody] DepartmentRequestResponseDto dto,
            [FromQuery] long deptId)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.UpdateDepartment(dto, deptId));
        }

        //--------------------------------------------------------------------------------------------------------------
        
        [HttpDelete(ApiEndpoints.DELETE_MASSIVE)]
        public async Task<ActionResult> DeleteMassiveDepartments([FromBody] MassiveDeleteRequestDto deleteDepartments)
        {
            await _service.DeleteMassiveDepartments(deleteDepartments, _helper
                .ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }

        //--------------------------------------------------------------------------------------------------------------

        [HttpDelete(ApiEndpoints.DELETE_ALL)]
        public async Task<ActionResult> DeleteAllDepartments()
        {
            await _service.DeleteAllDepartments(_helper.ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }
    }
}