using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("StudyRooms")]
public class StudyRoom : AbstractEntity
{
	[Required]
	[StringLength(50)]
	public string Name { get; set; }

	[Required]
	[StringLength(150)]
	public string Description { get; set; }

	[Required]
	public int Capacity { get; set; }

	[ForeignKey(nameof(Department))]
	public long DepartmentId { get; set; }

	[ForeignKey(nameof(Cathedral))]
	public long CathedralId { get; set; }

	[ForeignKey(nameof(RoomType))]
	public long RoomTypeId { get; set; }

	public virtual Department Department { get; set; }

	public virtual Cathedral Cathedral { get; set; }

	public virtual RoomType RoomType { get; set; }

	public virtual ICollection<ScheduleSubject> ScheduleSubjects { get; set; }
}
