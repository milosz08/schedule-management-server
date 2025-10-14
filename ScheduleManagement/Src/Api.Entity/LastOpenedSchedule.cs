using System.ComponentModel.DataAnnotations.Schema;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("LastOpenedSchedules")]
public class LastOpenedSchedule : AbstractEntity
{
	[ForeignKey(nameof(StudyGroup))] public long StudyGroupId { get; set; }

	[ForeignKey(nameof(Person))] public long? PersonId { get; set; }

	public virtual StudyGroup StudyGroup { get; set; }

	public virtual Person Person { get; set; }
}
