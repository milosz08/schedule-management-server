/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: StudyRoom.cs
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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("study-rooms")]
    public class StudyRoom : PrimaryKeyWithClientIdentifierInjection
    {
        [Required]
        [StringLength(50)]
        [Column("study-room-name")]
        public string Name { get; set; }
        
        [Required]
        [StringLength(150)]
        [Column("study-room-desc")]
        public string Description { get; set; }
        
        [Required]
        [Column("study-room-capacity")]
        public int Capacity { get; set; }
        
        [ForeignKey(nameof(Department))]
        [Column("department-key")]
        public long DepartmentId { get; set; }
        
        public virtual Department Department { get; set; }
        
        [ForeignKey(nameof(Cathedral))]
        [Column("cathedral-key")]
        public long CathedralId { get; set; }
        
        public virtual Cathedral Cathedral { get; set; }
        
        [ForeignKey(nameof(RoomType))]
        [Column("room-type-key")]
        public long RoomTypeId { get; set; }
        
        public virtual RoomType RoomType { get; set; }
        
        public virtual ICollection<ScheduleSubject> ScheduleSubjects { get; set; }
    }
}