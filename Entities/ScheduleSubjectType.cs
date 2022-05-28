using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("schedule-subject-types")]
    public class ScheduleSubjectType : PrimaryKeyWithClientIdentifierInjection
    {
        [Required]
        [StringLength(50)]
        [Column("schedule-type-name")]
        public string Name { get; set; }
        
        [Required]
        [StringLength(5)]
        [Column("schedule-type-alias")]
        public string Alias { get; set; }
        
        [Required]
        [StringLength(7)]
        [Column("schedule-type-color")]
        public string Color { get; set; }
    }
}