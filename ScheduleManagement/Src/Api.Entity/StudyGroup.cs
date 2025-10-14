using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("StudyGroups")]
public class StudyGroup : AbstractEntity
{
	[Required] [StringLength(50)] public string Name { get; set; }

	[ForeignKey(nameof(Department))] public long DepartmentId { get; set; }

	[ForeignKey(nameof(StudySpecialization))]
	public long StudySpecializationId { get; set; }

	[ForeignKey(nameof(Semester))] public long SemesterId { get; set; }

	public virtual Department Department { get; set; }

	public virtual StudySpecialization StudySpecialization { get; set; }

	public virtual Semester Semester { get; set; }

	public virtual ICollection<ScheduleSubject> ScheduleSubjects { get; set; }

	public virtual ICollection<ContactMessage> ContactMessages { get; set; }
}
