/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: TimeManagementController.cs
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
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;


namespace asp_net_po_schedule_management_server.Controllers
{
    /// <summary>
    /// Kontroler przechowujący metody odpowiadające za endpointy związane z czasem, datami i godzinami.
    /// </summary>
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    public sealed class TimeManagementController : ControllerBase
    {
        private readonly ITimeManagementService _service;
       
        //--------------------------------------------------------------------------------------------------------------
        
        public TimeManagementController(ITimeManagementService service)
        {
            _service = service;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_STUDY_YEARS)]
        public ActionResult<List<string>> GetAllStudyYearsFrom2020ToCurrent()
        {
            return StatusCode((int) HttpStatusCode.OK, _service.GetAllStudyYearsFrom2020ToCurrent());
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_WEEKSDATA_BASE_CURR_YEAR)]
        public ActionResult<List<string>> GetAllWeeksNameWithWeekNumberInCurrentYear(
            [FromQuery] int startYear, [FromQuery] int endYear)
        {
            return StatusCode((int) HttpStatusCode.OK, _service
                .GetAllWeeksNameWithWeekNumberInCurrentYear(startYear, endYear));
        }
    }
}