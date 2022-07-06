/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: IHelperService.cs
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
using System.Collections.Generic;

using asp_net_po_schedule_management_server.Dto;


namespace asp_net_po_schedule_management_server.Services
{
    public interface IHelperService
    {
        AvailablePaginationSizes GetAvailablePaginationTypes();
        Task<AvailableDataResponseDto<NameWithDbIdElement>> GetAvailableStudyTypes();
        Task<AvailableDataResponseDto<NameWithDbIdElement>> GetAvailableStudyDegreeTypes();
        Task<AvailableDataResponseDto<NameWithDbIdElement>> GetAvailableSemesters();
        Task<List<NameWithDbIdElement>> GetAvailableStudyDegreeBaseAllSpecs(long deptId);
        Task<List<NameWithDbIdElement>> GetAvailableSemBaseStudyGroups(long deptId, long studySpecId);
        Task<ConvertToNameWithIdResponseDto> ConvertNamesToIds(ConvertNamesToIdsRequestDto dto);
        Task<ConvertToNameWithIdResponseDto> ConvertIdsToNames(ConvertIdsToNamesRequestDto dto);
        Task<AvailableDataResponseDto<string>> GetAvailableSubjectTypes(string subjTypeName);
        Task<AvailableDataResponseDto<string>> GetAvailableRoomTypes();
        Task<AvailableDataResponseDto<string>> GetAvailableRoles();
    }
}