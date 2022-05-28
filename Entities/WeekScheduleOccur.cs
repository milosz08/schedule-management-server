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
        
        [ForeignKey(nameof(ScheduleSubject))]
        [Column("schedule-subject-key")]
        public long ScheduleSubjectId { get; set; }
        
        public virtual ScheduleSubject ScheduleSubject { get; set; }
    }
}