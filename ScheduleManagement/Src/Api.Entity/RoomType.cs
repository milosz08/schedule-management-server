using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ScheduleManagement.Api.Db;

namespace ScheduleManagement.Api.Entity;

[Table("RoomTypes")]
public sealed class RoomType : AbstractEntity
{
	[Required] [StringLength(50)] public string Name { get; set; }

	[Required] [StringLength(10)] public string Alias { get; set; }

	[Required] [StringLength(7)] public string Color { get; set; }
}
