/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: SearchContentController.cs
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

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Services;


namespace asp_net_po_schedule_management_server.Controllers
{
    /// <summary>
    /// Kontroler odpowiadający za masowe zapytania do API ze strony wyszukiwania treści. Kontroler niechroniony.
    /// </summary>
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    public sealed class SearchContentController : ControllerBase
    {
        private readonly ISearchContentService _service;
        
        //--------------------------------------------------------------------------------------------------------------
        
        public SearchContentController(ISearchContentService service)
        {
            _service = service;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        [HttpGet(ApiEndpoints.GET_MASSIVE_QUERY_RESULT)]
        public async Task<ActionResult<List<SearchMassiveQueryResDto>>> GetAllItemsFromMassiveServerQuery(
            [FromQuery] SearchMassiveQueryReqDto dto)
        { 
            return StatusCode((int) HttpStatusCode.OK, await _service.GetAllItemsFromMassiveServerQuery(dto));
        }
    }
}