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
    /// Kontroler przechowujący akcje do zarządzania encją sal zajęciowych w systemie. Umożliwia stworzenie sali,
    /// pobranie wszystkich wyników oraz zaawanowaną filtrację wyników i paginację, pobieranie sal i dodatkowe filtrowanie
    /// (po nazwie) oraz standardowe metody usuwające zawartość z bazy danych.
    /// </summary>
    [ApiController]
    [Route("/api/v1/dotnet/[controller]")]
    [AuthorizeRoles(AvailableRoles.ADMINISTRATOR)]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public sealed class StudyRoomsController : ControllerBase
    {
        private readonly IStudyRoomsService _service;
        private readonly ServiceHelper _helper;

        //--------------------------------------------------------------------------------------------------------------
        
        public StudyRoomsController(IStudyRoomsService service, ServiceHelper helper)
        {
            _service = service;
            _helper = helper;
        }

        //--------------------------------------------------------------------------------------------------------------
        
        [HttpPost(ApiEndpoints.ADD_STUDY_ROOM)]
        public async Task<ActionResult<CreateStudyRoomResponseDto>> AddNewStudyRoom(CreateStudyRoomRequestDto dto)
        {
            return StatusCode((int) HttpStatusCode.Created, await _service.CreateStudyRoom(dto));
        }
    }
}