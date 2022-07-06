/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: StudyGroupController.cs
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
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.CustomDecorators;
using asp_net_po_schedule_management_server.Services.Helpers;


namespace asp_net_po_schedule_management_server.Controllers
{
    /// <summary>
    /// Kontroler przechowujący akcje do zarządzania encją grup w systemie. Umożliwia stworzenie grupy, pobranie
    /// wszystkich wyników oraz zaawanowaną filtrację wyników i paginację oraz standardowe metody usuwające zawartość
    /// z bazy danych.
    /// </summary>
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    [AuthorizeRoles(AvailableRoles.ADMINISTRATOR)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public sealed class StudyGroupController : ControllerBase
    {
        private readonly ServiceHelper _helper;
        private readonly IStudyGroupService _service;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public StudyGroupController(IStudyGroupService service, ServiceHelper helper)
        {
            _service = service;
            _helper = helper;
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [HttpPost(ApiEndpoints.ADD_STUDY_GROUP)]
        public async Task<ActionResult<List<CreateStudyGroupResponseDto>>> AddNewStudyGroup(
            [FromBody] CreateStudyGroupRequestDto dto)
        {
            return StatusCode((int) HttpStatusCode.Created, await _service.CreateStudyGroup(dto));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_ALL_STUDY_GROUPS_PAGINATION)]
        public ActionResult<StudyGroupQueryResponseDto> GetStudyRooms([FromQuery] SearchQueryRequestDto searchSearchQuery)
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAllStudyGroups(searchSearchQuery));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [AllowAnonymous]
        [HttpGet(ApiEndpoints.GET_AVAILABLE_GROUPS_BASE_SPEC_AND_SEM_SCHEDULE)]
        public async Task<ActionResult<List<NameWithDbIdElement>>> GetAvailableStudyGroupsBaseSpecAndSem(
            [FromQuery] long studySpecId,
            [FromQuery] long semId)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service
                .GetAvailableGroupsBaseStudySpecAndSem(studySpecId, semId));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [AllowAnonymous]
        [HttpGet(ApiEndpoints.GET_AVAILABLE_GROUPS_BASE_SPEC)]
        public async Task<ActionResult<SearchQueryResponseDto>> GetAvailableStudyGroupsBaseSpec(
            [FromQuery] string groupName,
            [FromQuery] string deptName,
            [FromQuery] string studySpecName)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service
                .GetGroupsBaseStudySpec(groupName, deptName, studySpecName));
        }

        //--------------------------------------------------------------------------------------------------------------

        [AllowAnonymous]
        [HttpGet(ApiEndpoints.GET_ALL_GROUPS_BASE_DEPT)]
        public async Task<ActionResult<List<NameWithDbIdElement>>> GetAllStudyGroupsBaseDept([FromQuery] string deptName)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAllStudyGroupsBaseDept(deptName));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpDelete(ApiEndpoints.DELETE_MASSIVE)]
        public async Task<ActionResult> DeleteMassiveGroups([FromBody] MassiveDeleteRequestDto deleteGroups)
        {
            await _service.DeleteMassiveStudyGroups(deleteGroups, await _helper
                .ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }

        //--------------------------------------------------------------------------------------------------------------

        [HttpDelete(ApiEndpoints.DELETE_ALL)]
        public async Task<ActionResult> DeleteAllGroups()
        {
            await _service.DeleteAllStudyGroups(await _helper.ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }
    }
}