using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("ScheduleSubjectTypes")]
public class ScheduleSubjectType : AbstractEntity
{
	[Required]
	[StringLength(50)]
	public string Name { get; set; }

	[Required]
	[StringLength(5)]
	public string Alias { get; set; }

	[Required]
	[StringLength(7)]
	public string Color { get; set; }
}
