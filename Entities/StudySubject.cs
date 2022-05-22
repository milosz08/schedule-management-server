using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("study-subjects")]
    public class StudySubject : PrimaryKeyWithClientIdentifierInjection
    {
        [Required]
        [StringLength(50)]
        [Column("study-spec-name")]
        public string Name { get; set; }
        
        [Required]
        [StringLength(20)]
        [Column("study-spec-alias")]
        public string Alias { get; set; }
        
        [ForeignKey(nameof(Department))]
        [Column("dept-key")]
        public long DepartmentId { get; set; }
        
        public virtual Department Department { get; set; }
        
        [ForeignKey(nameof(StudySpecialization))]
        [Column("study-spec-key")]
        public long StudySpecializationId { get; set; }
        
        public virtual StudySpecialization StudySpecialization { get; set; }
        
        public virtual ICollection<Person> Users { get; set; }
    }
}