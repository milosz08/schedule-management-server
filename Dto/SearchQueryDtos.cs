/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: SearchQueryDtos.cs
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
using asp_net_po_schedule_management_server.Utils;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class SearchQueryRequestDto
    {
        public string SearchPhrase { get; set; } = "";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = ApplicationUtils._allowedPageSizes[0];
        public string SortBy { get; set; } = "Id";
        public SortDirection SortDirection { get; set; } = SortDirection.ASC;
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class SearchQueryResponseDto
    {
        public List<string> DataElements { get; set; }

        public SearchQueryResponseDto()
        {
            DataElements = new List<string>();
        }
        
        public SearchQueryResponseDto(List<string> dataElements)
        {
            DataElements = dataElements;
        }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public enum SortDirection
    {
        ASC, DES
    }
}