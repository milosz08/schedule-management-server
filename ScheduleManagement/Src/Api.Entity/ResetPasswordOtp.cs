using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("ResetPasswordOtp")]
public class ResetPasswordOtp : AbstractEntity
{
	[Required] [StringLength(50)] public string Email { get; set; }

	[Required] [StringLength(8)] public string Otp { get; set; }

	[Required] public DateTime OtpExpired { get; set; }

	[Required] public bool IfUsed { get; set; }

	[ForeignKey(nameof(Person))] public long PersonId { get; set; }

	public virtual Person Person { get; set; }
}
