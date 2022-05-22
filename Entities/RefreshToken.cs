using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("jwt-tokens")]
    public class RefreshToken : PrimaryKeyEntityInjection
    {
        [Required]
        [StringLength(200)]
        [Column("token-value")]
        public string TokenValue { get; set; }
        
        [ForeignKey(nameof(Person))]
        [Column("person-key")]
        public long PersonId { get; set; }
        
        public virtual Person Person { get; set; }
    }
}