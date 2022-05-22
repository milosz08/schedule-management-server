using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.CustomDecorators;

using asp_net_po_schedule_management_server.Dto.Responses;
using asp_net_po_schedule_management_server.Dto.RequestResponseMerged;


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
        private readonly ICathedralService _service;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public CathedralsController(ICathedralService service, ServiceHelper helper)
        {
            _service = service;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpPost(ApiEndpoints.ADD_CATHEDRAL)]
        public async Task<ActionResult<CreatedCathedralResponseDto>> CreateCathedral(
            [FromBody] CreateCathedralRequestDto dto)
        {
            return StatusCode((int) HttpStatusCode.Created, await _service.CreateCathedral(dto));
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [HttpGet(ApiEndpoints.GET_ALL_CATHEDRALS_BASE_DEPT)]
        public async Task<ActionResult<SearchQueryResponseDto>> GetAllCathedralsBasedDepartmentName(
            [FromQuery] string cathQuery,
            [FromQuery] string deptQuery)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service
                .GetAllCathedralsBasedDepartmentName(cathQuery, deptQuery));
        }
    }
}