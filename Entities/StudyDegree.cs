using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("study-degrees")]
    public sealed class StudyDegree : PrimaryKeyWithClientIdentifierInjection
    {
        [Required]
        [StringLength(50)]
        [Column("degree-name")]
        public string Name { get; set; }
        
        [Required]
        [StringLength(10)]
        [Column("degree-alias")]
        public string Alias { get; set; }
        
        [Required]
        [Column("degree-level")]
        public int Level { get; set; }
    }
}