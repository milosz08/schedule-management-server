using System.ComponentModel.DataAnnotations;

namespace ScheduleManagement.Api.Db;

public abstract class AbstractEntity
{
	[Key]
	public long Id { get; set; }

	public DateTime CreatedDate { get; set; }

	public DateTime UpdatedDate { get; set; }
}
