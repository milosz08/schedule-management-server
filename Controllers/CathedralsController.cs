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
        
        [HttpPut(ApiEndpoints.UPDATE_CATHEDRAL)]
        public async Task<ActionResult<CathedralResponseDto>> UpdateCathedral(
            [FromBody] CathedralRequestDto dto,
            [FromQuery] long cathId)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.UpdateCathedral(dto, cathId));
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
        
        //--------------------------------------------------------------------------------------------------------------
        
        [AllowAnonymous]
        [HttpGet(ApiEndpoints.GET_ALL_CATHEDRALS_SCHEDULE)]
        public ActionResult<List<NameWithDbIdElement>> GetAllCathedralsScheduleBaseDept(long deptId)
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAllCathedralsScheduleBaseDept(deptId));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_ALL_CATHEDRALS_PAGINATION)]
        public ActionResult<UserResponseDto> GetCathedrals([FromQuery] SearchQueryRequestDto searchSearchQuery)
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAllCathedrals(searchSearchQuery));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_CATHEDRAL_BASE_ID)]
        public async Task<ActionResult<CathedralEditResDto>> GetCathedralBaseDbId([FromQuery] long cathId)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetCathedralBaseDbId(cathId));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpDelete(ApiEndpoints.DELETE_MASSIVE)]
        public async Task<ActionResult> DeleteMassiveCathedrals([FromBody] MassiveDeleteRequestDto deleteCathedrals)
        {
            await _service.DeleteMassiveCathedrals(deleteCathedrals, _helper
                .ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }

        //--------------------------------------------------------------------------------------------------------------

        [HttpDelete(ApiEndpoints.DELETE_ALL)]
        public async Task<ActionResult> DeleteAllCathedrals()
        {
            await _service.DeleteAllCathedrals(_helper.ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }
    }
}