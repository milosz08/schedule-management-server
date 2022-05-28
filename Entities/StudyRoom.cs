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