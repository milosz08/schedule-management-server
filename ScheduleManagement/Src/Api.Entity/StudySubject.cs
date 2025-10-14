using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("StudySubjects")]
public class StudySubject : AbstractEntity
{
	[Required] [StringLength(50)] public string Name { get; set; }

	[Required] [StringLength(20)] public string Alias { get; set; }

	[ForeignKey(nameof(Department))] public long DepartmentId { get; set; }

	[ForeignKey(nameof(StudySpecialization))]
	public long StudySpecializationId { get; set; }

	public virtual Department Department { get; set; }

	public virtual StudySpecialization StudySpecialization { get; set; }

	public virtual ICollection<Person> Persons { get; set; }
}
