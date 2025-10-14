using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("StudySpecializations")]
public class StudySpecialization : AbstractEntity
{
	[Required] [StringLength(50)] public string Name { get; set; }

	[Required] [StringLength(20)] public string Alias { get; set; }

	[ForeignKey(nameof(StudyType))] public long StudyTypeId { get; set; }

	[ForeignKey(nameof(Department))] public long DepartmentId { get; set; }

	[ForeignKey(nameof(StudyDegree))] public long StudyDegreeId { get; set; }

	public virtual StudyType StudyType { get; set; }

	public virtual Department Department { get; set; }

	public virtual StudyDegree StudyDegree { get; set; }

	public virtual ICollection<Person> Students { get; set; }
}
