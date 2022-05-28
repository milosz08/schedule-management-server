using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("weekdays")]
    public sealed class Weekday : PrimaryKeyWithClientIdentifierInjection
    {
        [Required]
        [StringLength(50)]
        [Column("day-name")]
        public string Name { get; set; }
        
        [Required]
        [StringLength(3)]
        [Column("day-alias")]
        public string Alias { get; set; }
        
        [Required]
        [Column("day-identifier")]
        public int Number { get; set; }
    }
}