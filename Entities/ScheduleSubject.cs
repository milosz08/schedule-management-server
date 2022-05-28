using System;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("schedule-subjects")]
    public class ScheduleSubject : PrimaryKeyWithClientIdentifierInjection
    {
        [Required]
        [Column("start-time")]
        public TimeSpan StartTime { get; set; }
        
        [Required]
        [Column("end-time")]
        public TimeSpan EndTime { get; set; }

        [ForeignKey(nameof(StudyGroup))]
        [Column("group-key")]
        public long StudyGroupId { get; set; }
        
        public virtual StudyGroup StudyGroup { get; set; }
        
        [ForeignKey(nameof(Weekday))]
        [Column("weekday-key")]
        public long WeekdayId { get; set; }
        
        public virtual Weekday Weekday { get; set; }
        
        [ForeignKey(nameof(StudyType))]
        [Column("study-type-key")]
        public long StudyTypeId { get; set; }
        
        public virtual StudyType StudyType { get; set; }
        
        [ForeignKey(nameof(StudySubject))]
        [Column("subject-key")]
        public long StudySubjectId { get; set; }

        public virtual StudySubject StudySubject { get; set; }
        
        [ForeignKey(nameof(ScheduleSubjectType))]
        [Column("subject-type-key")]
        public long ScheduleSubjectTypeId { get; set; }

        public virtual ScheduleSubjectType ScheduleSubjectType { get; set; }
        
        public virtual ICollection<Person> ScheduleTeachers { get; set; }
        
        public virtual ICollection<StudyRoom> StudyRooms { get; set; }
    }
}