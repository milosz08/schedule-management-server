using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("Weekdays")]
public sealed class Weekday : AbstractEntity
{
	[Required]
	[StringLength(50)]
	public string Name { get; set; }

	[Required]
	[StringLength(3)]
	public string Alias { get; set; }

	[Required]
	public int Identifier { get; set; }
}
