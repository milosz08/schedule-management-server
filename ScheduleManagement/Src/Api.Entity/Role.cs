using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("Roles")]
public sealed class Role : AbstractEntity
{
	[Required]
	[MaxLength(50)]
	public string Name { get; set; }
}
