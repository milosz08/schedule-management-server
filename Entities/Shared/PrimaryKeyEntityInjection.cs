using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_net_po_schedule_management_server.Entities.Shared
{
    public abstract class PrimaryKeyEntityInjection
    {
        [Key]
        [Column("primary-key")]
        public long Id { get; set; }
        
        [Column("created-date")]
        public DateTime CreatedDate { get; set; }
        
        [Column("updated-date")]
        public DateTime UpdatedDate { get; set; }
    }
}