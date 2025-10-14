using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("WeekScheduleOccur")]
public class WeekScheduleOccur : AbstractEntity
{
	[Required] public byte WeekIdentifier { get; set; }

	[Required] public int Year { get; set; }

	[Required] public DateTime OccurDate { get; set; }

	[ForeignKey(nameof(ScheduleSubject))] public long ScheduleSubjectId { get; set; }

	public virtual ScheduleSubject ScheduleSubject { get; set; }
}
