using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("role")]
    public sealed class Role : PrimaryKeyWithClientIdentifierInjection
    {
        [Column("name")]
        [MaxLength(50)]
        public string Name { get; set; }
    }

    public enum AvailableRoles
    {
        TEACHER,
        EDITOR,
        ADMINISTRATOR,
    }
}