using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace asp_net_po_schedule_management_server.Entities.Shared
{
    public abstract class PrimaryKeyWithClientIdentifierInjection : PrimaryKeyEntityInjection
    {
        [StringLength(20, ErrorMessage = "Sztuczny index musi mieć 20 znaków")]
        [Column("artifician-index")]
        public string ArtifIndex { get; set; } = "123";
    }
}