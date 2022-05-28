using System.Net;
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
    /// Kontroler przechowujący akcje do zarządzania encją przedmiotów w systemie. Umożliwia stworzenie przedmiotu,
    /// pobranie wszystkich wyników oraz zaawanowaną filtrację wyników i paginację, pobieranie przedmiotów i dodatkowe
    /// filtrowanie (po nazwie) oraz standardowe metody usuwające zawartość z bazy danych.
    /// </summary>
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    [AuthorizeRoles(AvailableRoles.ADMINISTRATOR)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public sealed class StudySubjectController : ControllerBase
    {
        private readonly IStudySubjectService _service;
        private readonly ServiceHelper _helper;

        //--------------------------------------------------------------------------------------------------------------

        public StudySubjectController(IStudySubjectService service, ServiceHelper helper)
        {
            _helper = helper;
            _service = service;
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [HttpPost(ApiEndpoints.ADD_STUDY_SUBJECT)]
        public async Task<ActionResult<CreateStudySubjectResponseDto>> AddNewStudySubject(
            [FromBody] CreateStudySubjectRequestDto dto)
        {
            return StatusCode((int) HttpStatusCode.Created, await _service.AddNewStudySubject(dto));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_ALL_STUDY_SUBJECTS)]
        public ActionResult<StudySubjectQueryResponseDto> GetStudyRooms([FromQuery] SearchQueryRequestDto searchSearchQuery)
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAllStudySubjects(searchSearchQuery));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_ALL_STUDY_SUBJECT_BASE_DEPT)]
        public ActionResult<SearchQueryResponseDto> GetAllStudySubjectsBaseDept(
            [FromQuery] string subjcName,
            [FromQuery] long deptId,
            [FromQuery] long studySpecId)
        {
            return StatusCode((int) HttpStatusCode.OK, _service
                .GetAllStudySubjectsBaseDeptAndSpec(subjcName, deptId, studySpecId));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpDelete(ApiEndpoints.DELETE_MASSIVE)]
        public async Task<ActionResult> DeleteMassiveSubjects([FromBody] MassiveDeleteRequestDto deleteSubjects)
        {
            await _service.DeleteMassiveSubjects(deleteSubjects, _helper
                .ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }

        //--------------------------------------------------------------------------------------------------------------

        [HttpDelete(ApiEndpoints.DELETE_ALL)]
        public async Task<ActionResult> DeleteAllSubjects()
        {
            await _service.DeleteAllSubjects(_helper.ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }
    }
}