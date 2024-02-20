using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("ContactFormIssueTypes")]
public sealed class ContactFormIssueType : AbstractEntity
{
	[Required]
	[StringLength(50)]
	public string Name { get; set; }
}
