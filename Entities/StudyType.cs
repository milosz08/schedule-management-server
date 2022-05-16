using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("study-types")]
    public sealed class StudyType : PrimaryKeyWithClientIdentifierInjection
    {
        [Required]
        [Column("study-type-name")]
        public string Name { get; set; }
        
        [Required]
        [Column("study-type-alias")]
        public string Alias { get; set; }
    }
}