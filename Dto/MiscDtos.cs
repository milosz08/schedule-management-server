/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: MiscDtos.cs
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

using asp_net_po_schedule_management_server.Entities;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class MassiveDeleteRequestDto
    {
        public long[] ElementsIds { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class UserCredentialsHeaderDto
    {
        public string Login { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Person Person { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class AvailableDataResponseDto<T>
    {
        public List<T> DataElements { get; set; }
        
        public AvailableDataResponseDto(List<T> dataElements)
        {
            DataElements = dataElements;
        }
    }

    //------------------------------------------------------------------------------------------------------------------

    public sealed class NameWithDbIdElement
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public NameWithDbIdElement(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}