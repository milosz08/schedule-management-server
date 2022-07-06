﻿/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: ScheduleSubjectsController.cs
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
using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Services;
using asp_net_po_schedule_management_server.CustomDecorators;
using asp_net_po_schedule_management_server.Services.Helpers;


namespace asp_net_po_schedule_management_server.Controllers
{
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    [AuthorizeRoles(AvailableRoles.EDITOR, AvailableRoles.ADMINISTRATOR)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public sealed class ScheduleSubjectsController : ControllerBase
    {
        private readonly IScheduleSubjectsService _service;
        private readonly ServiceHelper _helper;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public ScheduleSubjectsController(IScheduleSubjectsService service, ServiceHelper helper)
        {
            _service = service;
            _helper = helper;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpPost(ApiEndpoints.ADD_SCHEDULE_ACTIVITY)]
        public async Task<ActionResult> AddNewScheduleActivity([FromBody] ScheduleActivityReqDto dto)
        {
            await _service.AddNewScheduleActivity(dto);
            return StatusCode((int) HttpStatusCode.Created);
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [AllowAnonymous]
        [HttpPost(ApiEndpoints.GET_SCHEDULE_SUBJECTS_BASE_GROUP_ID)]
        public async Task<ActionResult<ScheduleDataRes<ScheduleGroups>>> GetAllScheduleSubjectsBaseGroup(
            [FromQuery] ScheduleGroupQuery dto,
            [FromBody] ScheduleFilteringData filter)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAllScheduleSubjectsBaseGroup(dto, filter));
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [AllowAnonymous]
        [HttpPost(ApiEndpoints.GET_SCHEDULE_SUBJECTS_BASE_TEACHER_ID)]
        public async Task<ActionResult<ScheduleDataRes<ScheduleTeachers>>> GetAllScheduleSubjectsBaseTeacher(
            [FromQuery] ScheduleTeacherQuery dto,
            [FromBody] ScheduleFilteringData filter)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAllScheduleSubjectsBaseTeacher(dto, filter));
        }
        
        //--------------------------------------------------------------------------------------------------------------

        [AllowAnonymous]
        [HttpPost(ApiEndpoints.GET_SCHEDULE_SUBJECTS_BASE_ROOM_ID)]
        public async Task<ActionResult<ScheduleDataRes<ScheduleRooms>>> GetAllScheduleSubjectsBaseRoom(
            [FromQuery] ScheduleRoomQuery dto,
            [FromBody] ScheduleFilteringData filter)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAllScheduleSubjectsBaseRoom(dto, filter));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [AllowAnonymous]
        [HttpGet(ApiEndpoints.GET_SCHEDULE_SUBJECT_DETAILS)]
        public async Task<ActionResult<ScheduleSubjectDetailsResDto>> GetScheduleSubjectDetails([FromQuery] long schedSubjId)
        {
            return StatusCode((int) HttpStatusCode.OK, await _service.GetScheduleSubjectDetails(schedSubjId));
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpDelete(ApiEndpoints.DELETE_MASSIVE)]
        public async Task<ActionResult> DeleteMassiveGroups([FromBody] MassiveDeleteRequestDto deleteScheduleSubjects)
        {
            await _service.DeleteMassiveScheduleSubjects(deleteScheduleSubjects, await _helper
                .ExtractedUserCredentialsFromHeader(HttpContext, this.Request));
            return StatusCode((int) HttpStatusCode.NoContent);
        }
    }
}