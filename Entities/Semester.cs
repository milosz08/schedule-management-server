using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("semesters")]
    public sealed class Semester : PrimaryKeyWithClientIdentifierInjection
    {
        [Required]
        [StringLength(50)]
        [Column("sem-name")]
        public string Name { get; set; }
        
        [Required]
        [StringLength(10)]
        [Column("sem-alias")]
        public string Alias { get; set; }
    }
}