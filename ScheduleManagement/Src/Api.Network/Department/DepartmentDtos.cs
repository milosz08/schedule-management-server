using System.ComponentModel.DataAnnotations;

namespace ScheduleManagement.Api.Network.Department;

public class DepartmentRequestResponseDto
{
	[Required(ErrorMessage = "Pole nazwy wydziału nie może być puste")]
	public string Name { get; set; }

	[Required(ErrorMessage = "Pole aliasu nazwy wydziału nie może być puste")]
	public string Alias { get; set; }
}

public sealed class DepartmentQueryResponseDto : DepartmentRequestResponseDto
{
	public long Id { get; set; }
	public bool IsRemovable { get; set; }
}

public sealed class DepartmentEditResDto
{
	public string Name { get; set; }
	public string Alias { get; set; }
}
