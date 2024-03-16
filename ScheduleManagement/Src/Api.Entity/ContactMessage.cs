using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("ContactMessages")]
public class ContactMessage : AbstractEntity
{
	[StringLength(8)]
	public string MessageIdentifier { get; set; }

	[StringLength(50)]
	public string? AnonName { get; set; }

	[StringLength(50)]
	public string? AnonSurname { get; set; }

	[StringLength(100)]
	public string? AnonEmail { get; set; }

	[Required]
	[StringLength(300)]
	public string Description { get; set; }

	public bool IsAnonymous { get; set; }

	[ForeignKey(nameof(Department))]
	public long? DepartmentId { get; set; }

	[ForeignKey(nameof(Person))]
	public long? PersonId { get; set; }

	[ForeignKey(nameof(ContactFormIssueType))]
	public long ContactFormIssueTypeId { get; set; }

	public virtual Department? Department { get; set; }

	public virtual Person? Person { get; set; }

	public virtual ContactFormIssueType ContactFormIssueType { get; set; }

	public virtual ICollection<StudyGroup> StudyGroups { get; set; }
}

public static class ContactMessageRelationBuilder
{
	public static void BuildContactMessageRelations(this ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ContactMessage>()
			.HasMany(p => p.StudyGroups)
			.WithMany(p => p.ContactMessages)
			.UsingEntity<Dictionary<string, object>>("ContactMessagesGroupsBinding",
				b => b.HasOne<StudyGroup>().WithMany().HasForeignKey("GroupId"),
				b => b.HasOne<ContactMessage>().WithMany().HasForeignKey("ContactMessageId"));
	}
}
