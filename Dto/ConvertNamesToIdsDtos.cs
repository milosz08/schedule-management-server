/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: ConvertNamesToIdsDtos.cs
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

using asp_net_po_schedule_management_server.Entities;

namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class ConvertNamesToIdsRequestDto
    {
        public string DepartmentName { get; set; }
        public string StudySpecName { get; set; }
        public string StudyGroupName { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class ConvertIdsToNamesRequestDto
    {
        public long? DepartmentId { get; set; }
        public long? StudySpecId { get; set; }
        public long? StudyGroupId { get; set; }
    }

    //------------------------------------------------------------------------------------------------------------------

    public sealed class ConvertNamesToDataSingleElement
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public ConvertNamesToDataSingleElement(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class ConvertToNameWithIdResponseDto
    {
        public ConvertNamesToDataSingleElement DeptData { get; set; }
        public ConvertNamesToDataSingleElement StudySpecData { get; set; }
        public ConvertNamesToDataSingleElement StudyGroupData { get; set; }
        
        public ConvertToNameWithIdResponseDto(Department deptData, StudySpecialization specData, StudyGroup groupData)
        {
            DeptData = new ConvertNamesToDataSingleElement(deptData.Id, deptData.Name.ToLower());
            StudySpecData = new ConvertNamesToDataSingleElement(specData.Id, specData.Name.ToLower());
            StudyGroupData = new ConvertNamesToDataSingleElement(groupData.Id, groupData.Name.ToLower());
        }
    }
}