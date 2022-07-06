/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: IUsersService.cs
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

using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IUsersService
    {
        PaginationResponseDto<UserResponseDto> GetAllUsers(SearchQueryRequestDto searchQuery);
        Task<RegisterUpdateUserResponseDto> UpdateUserDetails(RegisterUpdateUserRequestDto dto, long userId, bool ifUpdateEmailPass);
        Task<List<NameWithDbIdElement>> GetAllEmployeersScheduleBaseCath(long deptId, long cathId);
        Task<List<NameWithDbIdElement>> GetAllTeachersScheduleBaseDeptAndSpec(long deptId, string subjName);
        Task<DashboardDetailsResDto> GetDashboardPanelData(Claim userIdentity);
        Task<UserDetailsEditResDto> GetUserBaseDbId(long userId);
        Task DeleteMassiveUsers(MassiveDeleteRequestDto users, UserCredentialsHeaderDto credentials);
        Task DeleteAllUsers(UserCredentialsHeaderDto credentials);
    }
}