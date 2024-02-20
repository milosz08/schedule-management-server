using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("Persons")]
public class Person : AbstractEntity
{
	[Required]
	[StringLength(50)]
	public string Name { get; set; }

	[Required]
	[StringLength(50)]
	public string Surname { get; set; }

	[Required]
	[StringLength(8)]
	public string Shortcut { get; set; }

	[Required]
	[StringLength(50)]
	public string Login { get; set; }

	[Required]
	public bool FirstAccess { get; set; } = true;

	[Required]
	[StringLength(500)]
	public string Password { get; set; }

	[Required]
	[StringLength(100)]
	public string Email { get; set; }

	[Required]
	[StringLength(100)]
	public string Nationality { get; set; }

	[Required]
	[StringLength(100)]
	public string City { get; set; }

	[Required]
	[StringLength(20)]
	public string EmailPassword { get; set; }

	[Required]
	public bool HasPicture { get; set; }

	[Required]
	public bool IfRemovable { get; set; } = true;

	[ForeignKey(nameof(Role))]
	public long RoleId { get; set; }

	[ForeignKey(nameof(Department))]
	public long? DepartmentId { get; set; }

	[ForeignKey(nameof(Cathedral))]
	public long? CathedralId { get; set; }

	public virtual Role Role { get; set; }

	public virtual Department? Department { get; set; }

	public virtual Cathedral? Cathedral { get; set; }

	public virtual ICollection<StudySpecialization> StudySpecializations { get; set; }

	public virtual ICollection<StudySubject> Subjects { get; set; }

	public virtual ICollection<ScheduleSubject> ScheduleSubjects { get; set; }
}

public static class PersonRelationBuilder
{
	public static void BuildPersonRelations(this ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Person>()
			.HasMany(p => p.StudySpecializations)
			.WithMany(p => p.Students)
			.UsingEntity<Dictionary<string, object>>("StudentsSpecsBinding",
				b => b.HasOne<StudySpecialization>().WithMany().HasForeignKey("StudySpecId"),
				b => b.HasOne<Person>().WithMany().HasForeignKey("StudentId"));

		modelBuilder.Entity<Person>()
			.HasMany(p => p.Subjects)
			.WithMany(p => p.Persons)
			.UsingEntity<Dictionary<string, object>>("UsersSubjectsBinding",
				b => b.HasOne<StudySubject>().WithMany().HasForeignKey("StudySubjectId"),
				b => b.HasOne<Person>().WithMany().HasForeignKey("PersonId"));
	}
}
