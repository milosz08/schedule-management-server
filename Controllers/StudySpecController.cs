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
    /// Kontroler przechowujący akcje do zarządzania encją kierunków studiów w systemie. Umożliwia stworzenie kierunku,
    /// pobranie wszystkich wyników oraz zaawanowaną filtrację wyników i paginację, pobieranie kierunków i dodatkowe
    /// filtrowanie (po nazwie) oraz standardowe metody usuwające zawartość z bazy danych.
    /// </summary>
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    [AuthorizeRoles(AvailableRoles.ADMINISTRATOR)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public sealed class StudySpecController : ControllerBase
    {
        private readonly IStudySpecService _service;
        private readonly ServiceHelper _helper;

        //--------------------------------------------------------------------------------------------------------------

        public StudySpecController(IStudySpecService service, ServiceHelper helper)
        {
            _service = service;
            _helper = helper;
        }

        //--------------------------------------------------------------------------------------------------------------

        [HttpPost(ApiEndpoints.ADD_STUDY_SPECIALIZATION)]
        public async Task<ActionResult<IEnumerable<CreateStudySpecResponseDto>>> AddNewStudySpecialization(
            [FromBody] CreateStudySpecRequestDto dto)
        {
            return StatusCode((int) HttpStatusCode.Created, await _service.AddNewStudySpecialization(dto));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_All_STUDY_SPEC_BASE_DEPT)]
        public ActionResult<SearchQueryResponseDto> GetAllStudySpecializationsBaseDept(
            [FromQuery] string specQuery,
            [FromQuery] string deptQuery)
        {
            return StatusCode((int) HttpStatusCode.OK, _service
                .GetAllStudySpecializationsInDepartment(specQuery, deptQuery));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_ALL_STUDY_SPECS)]
        public ActionResult<StudySpecQueryResponseDto> GetAllStudySpecializations(
            [FromQuery] SearchQueryRequestDto searchSearchQuery)
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAllStudySpecializations(searchSearchQuery));
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [AllowAnonymous]
        [HttpGet(ApiEndpoints.GET_ALL_STUDY_SPECS_SCHEDULE)]
        public async Task<ActionResult<NameWithDbIdElement>> GetAllStudySpecsScheduleBaseDept([FromQuery] long deptId)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAllStudySpecsScheduleBaseDept(deptId));
        }

        //--------------------------------------------------------------------------------------------------------------
        
        [HttpDelete(ApiEndpoints.DELETE_MASSIVE)]
        public async Task<ActionResult> DeleteMassiveStudySpecs([FromBody] MassiveDeleteRequestDto deleteSpecs)
        {
            await _service.DeleteMassiveStudySpecs(deleteSpecs, _helper
                .ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }

        //--------------------------------------------------------------------------------------------------------------

        [HttpDelete(ApiEndpoints.DELETE_ALL)]
        public async Task<ActionResult> DeleteAllStudySpecs()
        {
            await _service.DeleteAllStudySpecs(_helper.ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }
    }
}