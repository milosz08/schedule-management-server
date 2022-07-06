/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: StudySubjectController.cs
 * Project name | Nazwa Projektu: asp-net-po-schedule-management-server
 *
 * Klient | Client: <https://github.com/Milosz08/Angular_PO_Schedule_Management_Client>
 * Serwer | Server: <https://github.com/Milosz08/ASP.NET_PO_Schedule_Management_Server>
 *
 * RestAPI for the Angular application to manage schedule for sample university. Written with the ASP.NET Core
 * and Entity Framework with mySQL database. Project for the teaching course "Objected Oriented Programming".
 *
 * RestAPI dla aplikacji Angular do zarządzania planem zajęć przykładowej uczelni wyższej. Napisane w oparciu o
 * ASP.NET Core oraz Entity Framework z bazą danych mySQL. Projekt wykonany na zajęcia "Programowanie Obiektowe".
 */

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
        public async Task<ActionResult<StudySubjectResponseDto>> AddNewStudySubject(
            [FromBody] StudySubjectRequestDto dto)
        {
            return StatusCode((int) HttpStatusCode.Created, await _service.AddNewStudySubject(dto));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpPut(ApiEndpoints.UPDATE_STUDY_SUBJECT)]
        public async Task<ActionResult<StudySubjectResponseDto>> UpdateStudySubject(
            [FromBody] StudySubjectRequestDto dto,
            [FromQuery] long subjId)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.UpdateStudySubject(dto, subjId));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [AllowAnonymous]
        [HttpGet(ApiEndpoints.GET_ALL_STUDY_SUBJECTS)]
        public ActionResult<StudySubjectQueryResponseDto> GetStudyRooms([FromQuery] SearchQueryRequestDto searchSearchQuery)
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAllStudySubjects(searchSearchQuery));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [AllowAnonymous]
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
        
        [AllowAnonymous]
        [HttpGet(ApiEndpoints.GET_AVAILABLE_SUBJECTS_BASE_DEPT)]
        public async Task<ActionResult<AvailableDataResponseDto<NameWithDbIdElement>>> GetAvailableStudySubjectsBaseDept(
            [FromQuery] string deptName)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAvailableSubjectsBaseDept(deptName));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_STUDY_SUBJECT_BASE_ID)]
        public async Task<ActionResult<StudySubjectEditResDto>> GetStudySubjectBaseDbId([FromQuery] long subjId)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetStudySubjectBaseDbId(subjId));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpDelete(ApiEndpoints.DELETE_MASSIVE)]
        public async Task<ActionResult> DeleteMassiveSubjects([FromBody] MassiveDeleteRequestDto deleteSubjects)
        {
            await _service.DeleteMassiveSubjects(deleteSubjects, await _helper
                .ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }

        //--------------------------------------------------------------------------------------------------------------

        [HttpDelete(ApiEndpoints.DELETE_ALL)]
        public async Task<ActionResult> DeleteAllSubjects()
        {
            await _service.DeleteAllSubjects(await _helper.ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }
    }
}