using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("person")]
    public class Person : PrimaryKeyWithClientIdentifierInjection
    {
        [Required]
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; }
        
        [Required]
        [Column("surname")]
        [StringLength(50)]
        public string Surname { get; set; }
        
        [Required]
        [Column("shortcut")]
        [StringLength(8)]
        public string Shortcut { get; set; }
        
        [Required]
        [Column("login")]
        [StringLength(50)]
        public string Login { get; set; }
        
        [Required]
        [Column("first-access")]
        public bool FirstAccess { get; set; } = true;
        
        [Required]
        [Column("password")]
        [StringLength(500)]
        public string Password { get; set; }
        
        [Required]
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; }
        
        [Required]
        [Column("nationality")]
        [StringLength(100)]
        public string Nationality { get; set; }

        [Required]
        [Column("city")]
        [StringLength(100)]
        public string City { get; set; }
        
        [Required]
        [StringLength(20)]
        [Column("email-password")]
        public string EmailPassword { get; set; }
        
        [Required]
        [Column("has-picture")]
        public bool HasPicture { get; set; }
        
        [Required]
        [Column("if-removable")]
        public bool IfRemovable { get; set; } = true;
        
        [ForeignKey(nameof(Role))]
        [Column("role-key")]
        public long RoleId { get; set; }
        
        public virtual Role Role { get; set; }
        
        [ForeignKey(nameof(Department))]
        [Column("dept-key")]
        public long? DepartmentId { get; set; }
        
        public virtual Department Department { get; set; }
        
        [ForeignKey(nameof(Cathedral))]
        [Column("cath-key")]
        public long? CathedralId { get; set; }
        
        public virtual Cathedral Cathedral { get; set; }

        public virtual ICollection<StudySpecialization> StudySpecializations { get; set; }
        
        public virtual ICollection<StudySubject> Subjects { get; set; }
        
        public virtual ICollection<ScheduleSubject> ScheduleSubjects { get; set; }
    }
}