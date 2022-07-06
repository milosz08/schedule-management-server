/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: WeekScheduleOccur.cs
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

using System;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("week-schedule-occur")]
    public class WeekScheduleOccur : PrimaryKeyWithClientIdentifierInjection
    {
        [Required]
        [Column("week-identifier")]
        public byte WeekNumber { get; set; }
        
        [Required]
        [Column("week-year")]
        public int Year { get; set; }
        
        [Required]
        [Column("week-occur")]
        public DateTime OccurDate { get; set; }
        
        [ForeignKey(nameof(ScheduleSubject))]
        [Column("schedule-subject-key")]
        public long ScheduleSubjectId { get; set; }
        
        public virtual ScheduleSubject ScheduleSubject { get; set; }
    }
}