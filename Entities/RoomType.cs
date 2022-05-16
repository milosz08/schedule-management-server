﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("room-types")]
    public sealed class RoomType : PrimaryKeyWithClientIdentifierInjection
    {
        [Column("room-type-name")]
        [StringLength(50)]
        public string Name { get; set; }
        
        [StringLength(10)]
        [Column("room-type-alias")]
        public string Alias { get; set; }
        
        [StringLength(6)]
        [Column("room-type-color")]
        public string Color { get; set; }
    }
}