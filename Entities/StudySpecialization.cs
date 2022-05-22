using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("study-specializations")]
    public class StudySpecialization : PrimaryKeyWithClientIdentifierInjection
    {
        [Required]
        [StringLength(50)]
        [Column("study-spec-name")]
        public string Name { get; set; }
        
        [Required]
        [StringLength(20)]
        [Column("study-spec-alias")]
        public string Alias { get; set; }
        
        [ForeignKey(nameof(StudyType))]
        [Column("study-type-key")]
        public long StudyTypeId { get; set; }
        
        public virtual StudyType StudyType { get; set; }
        
        [ForeignKey(nameof(Department))]
        [Column("dept-key")]
        public long DepartmentId { get; set; }
        
        public virtual Department Department { get; set; }
        
        [ForeignKey(nameof(StudyDegree))]
        [Column("degree-key")]
        public long StudyDegreeId { get; set; }
        
        public virtual StudyDegree StudyDegree { get; set; }
        
        public virtual ICollection<Person> Students { get; set; }
    }

    //------------------------------------------------------------------------------------------------------------------
    
    public static class StudySpecTypes
    {
        public static string ALL { get; set; } = "ALL";
        public static string ST { get; set; } = "ST";
        public static string NSZ { get; set; } = "NS/Z";
    }
}