using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using asp_net_po_schedule_management_server.Entities.Shared;


namespace asp_net_po_schedule_management_server.Entities
{
    [Table("role")]
    public sealed class Role : PrimaryKeyWithClientIdentifierInjection
    {
        [Required]
        [Column("name")]
        [MaxLength(50)]
        public string Name { get; set; }
    }

    public static class AvailableRoles
    {
        public const string STUDENT = "student";
        public const string TEACHER = "nauczyciel";
        public const string EDITOR = "edytor";
        public const string ADMINISTRATOR = "administrator";
    }
}