using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("Cathedrals")]
public class Cathedral : AbstractEntity
{
	[Required] [StringLength(100)] public string Name { get; set; }

	[Required] [StringLength(20)] public string Alias { get; set; }

	[Required] public bool IsRemovable { get; set; } = true;

	[ForeignKey(nameof(Department))] public long DepartmentId { get; set; }

	public virtual Department Department { get; set; }
}
