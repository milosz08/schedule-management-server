/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: StudyRoomDtos.cs
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

using System.ComponentModel.DataAnnotations;


namespace asp_net_po_schedule_management_server.Dto
{
    public sealed class StudyRoomRequestDto
    {
        [Required(ErrorMessage = "Pole nazwy (aliasu) sali nie może być puste")]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Pole nazwy wydziału nie może być puste")]
        public string DepartmentName { get; set; }
        
        [Required(ErrorMessage = "Pole nazwy katedry nie może być puste")]
        public string CathedralName { get; set; }
        
        [Required(ErrorMessage = "Pole pojemności sali nie może być puste")]
        public int Capacity { get; set; }
        
        [Required(ErrorMessage = "Pole typu sali nie może być puste")]
        public string RoomTypeName { get; set; }
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class StudyRoomResponseDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public string DepartmentFullName { get; set; }
        public string CathedralFullName { get; set; }
        public string RoomTypeFullName { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class StudyRoomQueryResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } 
        public int Capacity { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentAlias { get; set; }
        public string CathedralName { get; set; }
        public string CathedralAlias { get; set; }
        public string DeptWithCathAlias { get; set; }
        public string RoomTypeName { get; set; }
        public string RoomTypeAlias { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class StudyRoomEditResDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DepartmentName { get; set; }
        public string CathedralName { get; set; }
        public int Capacity { get; set; }
        public string RoomTypeName { get; set; }
    }
}