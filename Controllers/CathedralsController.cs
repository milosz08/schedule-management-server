using System.Net;
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
    /// Kontroler przechowujący akcje do zarządzania encją katedr w systemie. Umożliwia stworzenie katedry, pobranie
    /// wszystkich wyników oraz zaawanowaną filtrację wyników i paginację, pobieranie katedr konkretnych wydziałów,
    /// pobieranie katedr wydziałów i dodatkowe filtrowanie oraz standardowe metody usuwające zawartość z bazy danych.
    /// </summary>
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    [AuthorizeRoles(AvailableRoles.ADMINISTRATOR)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public sealed class CathedralsController : ControllerBase
    {
        private readonly ServiceHelper _helper;
        private readonly ICathedralService _service;

        //--------------------------------------------------------------------------------------------------------------
        
        public CathedralsController(ICathedralService service, ServiceHelper helper)
        {
            _service = service;
            _helper = helper;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpPost(ApiEndpoints.ADD_CATHEDRAL)]
        public async Task<ActionResult<CathedralResponseDto>> CreateCathedral([FromBody] CathedralRequestDto dto)
        {
            return StatusCode((int) HttpStatusCode.Created, await _service.CreateCathedral(dto));
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [HttpGet(ApiEndpoints.GET_ALL_CATHEDRALS_BASE_DEPT)]
        public ActionResult<SearchQueryResponseDto> GetAllCathedralsBasedDepartmentName(
            [FromQuery] string cathQuery,
            [FromQuery] string deptQuery)
        {
            return StatusCode((int) HttpStatusCode.OK, _service
                .GetAllCathedralsBasedDepartmentName(cathQuery, deptQuery));
        }
    }
}