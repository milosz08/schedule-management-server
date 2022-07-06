/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: HelperController.cs
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

using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.CustomDecorators;


namespace asp_net_po_schedule_management_server.Controllers
{
    /// <summary>
    /// Kontroler niechroniony (endpointy dostępne bez użycia JWT) przechowujący metody służące głównie do pobierania
    /// danych z bazy danych (np. w select boxach czy combo boxach na front-endzie).
    /// </summary>
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    public sealed class HelperController : ControllerBase
    {
        private readonly IHelperService _service;

        //--------------------------------------------------------------------------------------------------------------
        
        public HelperController(IHelperService service)
        {
            _service = service;
        }

        //--------------------------------------------------------------------------------------------------------------

        [HttpGet(ApiEndpoints.GET_AVAILABLE_STUDY_TYPES)]
        public async Task<ActionResult<AvailableDataResponseDto<NameWithDbIdElement>>> GetAvailableStudyTypes()
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAvailableStudyTypes());
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_AVAILABLE_STUDY_DEGREES)]
        public async Task<ActionResult<AvailableDataResponseDto<NameWithDbIdElement>>> GetAvailableStudyDegreeTypes()
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAvailableStudyDegreeTypes());
        }

        //--------------------------------------------------------------------------------------------------------------

        [HttpGet(ApiEndpoints.GET_AVAILABLE_DEGREES_BASE_STUDY_SPEC)]
        public async Task<ActionResult<List<NameWithDbIdElement>>> GetAvailableDegreesBaseStudySpec(
            [FromQuery] long deptId)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAvailableStudyDegreeBaseAllSpecs(deptId));
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [HttpGet(ApiEndpoints.GET_AVAILABLE_SEM_BASE_STUDY_GROUP)]
        public async Task<ActionResult<List<NameWithDbIdElement>>> GetAvailableSemBaseStudyGroup(
            [FromQuery] long deptId,
            [FromQuery] long studySpecId)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAvailableSemBaseStudyGroups(deptId, studySpecId));
        }

        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_AVAILABLE_SEMESTERS)]
        public async Task<ActionResult<AvailableDataResponseDto<NameWithDbIdElement>>> GetAvailableSemesters()
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAvailableSemesters());
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_AVAILABLE_PAGINATIONS)]
        public ActionResult<AvailablePaginationSizes> GetAvailablePaginationSizes()
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAvailablePaginationTypes());
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [HttpGet(ApiEndpoints.GET_AVAILABLE_ROOM_TYPES)]
        public async Task<ActionResult<AvailableDataResponseDto<string>>> GetAvailableRoomTypes()
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAvailableRoomTypes());
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [HttpGet(ApiEndpoints.GET_AVAILABLE_ROLES)]
        public async Task<ActionResult<AvailableDataResponseDto<string>>> GetAvailableRoles()
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAvailableRoles());
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [HttpGet(ApiEndpoints.GET_AVAILABLE_SUBJECT_TYPES)]
        public async Task<ActionResult<AvailableDataResponseDto<string>>> GetAvailableSubjectTypes(
            [FromQuery] string subjTypeName)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAvailableSubjectTypes(subjTypeName));
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [AuthorizeRoles(AvailableRoles.ADMINISTRATOR, AvailableRoles.EDITOR)]
        [HttpPost(ApiEndpoints.CONVERT_SCHEDULE_DATA_NAMES_TO_IDS)]
        public async Task<ActionResult<ConvertToNameWithIdResponseDto>> ConvertNamesToIds(ConvertNamesToIdsRequestDto dto)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.ConvertNamesToIds(dto));
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [AuthorizeRoles(AvailableRoles.ADMINISTRATOR, AvailableRoles.EDITOR)]
        [HttpPost(ApiEndpoints.CONVERT_SCHEDULE_DATA_IDS_TO_NAMES)]
        public async Task<ActionResult<ConvertToNameWithIdResponseDto>> ConvertIdsToNames(ConvertIdsToNamesRequestDto dto)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.ConvertIdsToNames(dto));
        }
    }
}