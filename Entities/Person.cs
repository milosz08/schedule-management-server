using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_net_po_schedule_management_server.Entities
{
    [Table("persons-table")]
    public class Person
    {
        [Key]
        [Column("primary-key")]
        public int Id { get; set; }
        
        [Column("person-identifier")]
        public string PersonIdentifier { get; set; }
        
        [Column("person-shortcut")]
        public string PersonShortcut { get; set; }
        
        [Column("app-login")]
        public string AppLogin { get; set; }
        
        [Column("app-password")]
        public string AppPassword { get; set; }
    }
}