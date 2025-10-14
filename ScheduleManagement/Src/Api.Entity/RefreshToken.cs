using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("JwtTokens")]
public class RefreshToken : AbstractEntity
{
	[Required] [StringLength(200)] public string Token { get; set; }

	[ForeignKey(nameof(Person))] public long PersonId { get; set; }

	public virtual Person Person { get; set; }
}
