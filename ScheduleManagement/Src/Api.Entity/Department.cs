using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("Departments")]
public sealed class Department : AbstractEntity
{
	[Required] [StringLength(100)] public string Name { get; set; }

	[Required] [StringLength(20)] public string Alias { get; set; }

	[Required] public bool IsRemovable { get; set; } = true;
}
