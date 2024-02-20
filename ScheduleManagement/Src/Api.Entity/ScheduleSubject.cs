using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("ScheduleSubjects")]
public class ScheduleSubject : AbstractEntity
{
	[Required]
	public TimeSpan StartTime { get; set; }

	[Required]
	public TimeSpan EndTime { get; set; }

	[Required]
	[StringLength(5)]
	public string StudyYear { get; set; }

	[ForeignKey(nameof(Weekday))]
	public long WeekdayId { get; set; }

	[ForeignKey(nameof(StudySubject))]
	public long StudySubjectId { get; set; }

	[ForeignKey(nameof(ScheduleSubjectType))]
	public long ScheduleSubjectTypeId { get; set; }

	public virtual Weekday Weekday { get; set; }

	public virtual StudySubject StudySubject { get; set; }

	public virtual ScheduleSubjectType ScheduleSubjectType { get; set; }

	public virtual ICollection<Person> ScheduleTeachers { get; set; }

	public virtual ICollection<StudyRoom> StudyRooms { get; set; }

	public virtual ICollection<WeekScheduleOccur> WeekScheduleOccurs { get; set; }

	public virtual ICollection<StudyGroup> StudyGroups { get; set; }
}

public static class ScheduleSubjectRelationBuilder
{
	public static void BuildScheduleSubjectRelations(this ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ScheduleSubject>()
			.HasMany(p => p.ScheduleTeachers)
			.WithMany(p => p.ScheduleSubjects)
			.UsingEntity<Dictionary<string, object>>("ScheduleTeachersBinding",
				b => b.HasOne<Person>().WithMany().HasForeignKey("TeacherId"),
				b => b.HasOne<ScheduleSubject>().WithMany().HasForeignKey("ScheduleSubjectId"));

		modelBuilder.Entity<ScheduleSubject>()
			.HasMany(p => p.StudyRooms)
			.WithMany(p => p.ScheduleSubjects)
			.UsingEntity<Dictionary<string, object>>("ScheduleRoomsBinding",
				b => b.HasOne<StudyRoom>().WithMany().HasForeignKey("RoomId"),
				b => b.HasOne<ScheduleSubject>().WithMany().HasForeignKey("ScheduleSubjectId"));

		modelBuilder.Entity<ScheduleSubject>()
			.HasMany(p => p.StudyGroups)
			.WithMany(p => p.ScheduleSubjects)
			.UsingEntity<Dictionary<string, object>>("ScheduleGroupsBinding",
				b => b.HasOne<StudyGroup>().WithMany().HasForeignKey("GroupId"),
				b => b.HasOne<ScheduleSubject>().WithMany().HasForeignKey("ScheduleSubjectId"));
	}
}
