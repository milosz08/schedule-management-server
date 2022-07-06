/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: ResetPasswordOtp.cs
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
    [Table("reset-password-otp")]
    public class ResetPasswordOtp : PrimaryKeyEntityInjection
    {
        [Required]
        [StringLength(50)]
        [Column("user-email")]
        public string Email { get; set; }
        
        [Required]
        [StringLength(8)]
        [Column("user-otp")]
        public string Otp { get; set; }
        
        [Required]
        [Column("otp-expired")]
        public DateTime OtpExpired { get; set; }
        
        [Required]
        [Column("if-used")]
        public bool IfUsed { get; set; }
        
        [ForeignKey(nameof(Person))]
        [Column("person-key")]
        public long PersonId { get; set; }
        
        public virtual Person Person { get; set; }
    }
}