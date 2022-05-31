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
        
        public ICollection<ScheduleSubject> ScheduleSubjects { get; set; }
    }
}