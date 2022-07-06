/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: StudyGroup.cs
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
    [Table("study-groups")]
    public class StudyGroup : PrimaryKeyWithClientIdentifierInjection
    {
        [Required]
        [StringLength(50)]
        [Column("group-name")]
        public string Name { get; set; }
        
        [ForeignKey(nameof(Department))]
        [Column("dept-key")]
        public long DepartmentId { get; set; }
        
        public virtual Department Department { get; set; }
        
        [ForeignKey(nameof(StudySpecialization))]
        [Column("study-spec-key")]
        public long StudySpecializationId { get; set; }
        
        public virtual StudySpecialization StudySpecialization { get; set; }
        
        [ForeignKey(nameof(Semester))]
        [Column("sem-key")]
        public long SemesterId { get; set; }
        
        public virtual Semester Semester { get; set; }
        
        public virtual ICollection<ScheduleSubject> ScheduleSubjects { get; set; }
        
        public virtual ICollection<ContactMessage> ContactMessages { get; set; }
    }
}