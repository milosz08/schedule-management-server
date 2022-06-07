using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("contact-messages")]
    public class ContactMessage : PrimaryKeyWithClientIdentifierInjection
    {
        [StringLength(8)]
        [Column("mess-identifier")]
        public string MessageIdentifier { get; set; }
        
        [StringLength(50)]
        [Column("anonymous-name")]
        public string Name { get; set; }
        
        [StringLength(50)]
        [Column("anonymous-surname")]
        public string Surname { get; set; }
        
        [StringLength(100)]
        [Column("anonymous-email")]
        public string Email { get; set; }
        
        [Required]
        [StringLength(300)]
        [Column("description")]
        public string Description { get; set; }
        
        [Column("if-anonymous")]
        public bool IfAnonymous { get; set; }
        
        [ForeignKey(nameof(Department))]
        [Column("dept-key")]
        public long? DepartmentId { get; set; }
        
        public virtual Department Department { get; set; }
        
        [ForeignKey(nameof(Person))]
        [Column("user-key")]
        public long? PersonId { get; set; }
        
        public virtual Person Person { get; set; }
        
        [ForeignKey(nameof(ContactFormIssueType))]
        [Column("issue-type-key")]
        public long ContactFormIssueTypeId { get; set; }
        
        public virtual ContactFormIssueType ContactFormIssueType { get; set; }
        
        public virtual ICollection<StudyGroup> StudyGroups { get; set; }
    }
}