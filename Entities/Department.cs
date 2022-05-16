using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("departments")]
    public sealed class Department : PrimaryKeyWithClientIdentifierInjection
    {
        [Required]
        [Column("dept-name")]
        public string Name { get; set; }
        
        [Required]
        [Column("dept-alias")]
        public string Alias { get; set; }
    }
}