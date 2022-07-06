/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: CathedralDtos.cs
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
    public class CathedralRequestDto
    {
        [Required(ErrorMessage = "Pole nazwy katedry nie może być puste")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Pole aliasu katedry nie może być puste")]
        public string Alias { get; set; }
        
        [Required(ErrorMessage = "Pole przypisanego wydziału do katedry nie może być puste")]
        public string DepartmentName { get; set; }
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class CathedralResponseDto
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string DepartmentFullName { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------
    
    public sealed class CathedralQueryResponseDto : CathedralRequestDto
    {
        public long Id { get; set; }
        public string DepartmentAlias { get; set; }
        public bool IfRemovable { get; set; }
    }
    
    //------------------------------------------------------------------------------------------------------------------

    public sealed class CathedralEditResDto
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string DepartmentName { get; set; }
    }
}