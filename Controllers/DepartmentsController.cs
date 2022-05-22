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
        
        [HttpGet(ApiEndpoints.GET_ALL_DEPARTMENTS_LIST)]
        public ActionResult<SearchQueryResponseDto> GetAllDepartmentsList([FromQuery] string deptQuerySearch)
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAllDepartmentsList(deptQuerySearch));
        }
    }
}