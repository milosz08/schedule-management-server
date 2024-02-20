using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("StudyTypes")]
public sealed class StudyType : AbstractEntity
{
	[Required]
	[StringLength(50)]
	public string Name { get; set; }

	[Required]
	[StringLength(20)]
	public string Alias { get; set; }
}
