using System.ComponentModel.DataAnnotations;

namespace ScheduleManagement.Api.Network.Cathedral;

public class CathedralRequestDto
{
	[Required(ErrorMessage = "Pole nazwy katedry nie może być puste")]
	public string Name { get; set; }

	[Required(ErrorMessage = "Pole aliasu katedry nie może być puste")]
	public string Alias { get; set; }

	[Required(ErrorMessage = "Pole przypisanego wydziału do katedry nie może być puste")]
	public string DepartmentName { get; set; }
}

public sealed class CathedralResponseDto
{
	public string Name { get; set; }
	public string Alias { get; set; }
	public string DepartmentFullName { get; set; }
}

public sealed class CathedralQueryResponseDto : CathedralRequestDto
{
	public long Id { get; set; }
	public string DepartmentAlias { get; set; }
	public bool IsRemovable { get; set; }
}

public sealed class CathedralEditResDto
{
	public string Name { get; set; }
	public string Alias { get; set; }
	public string DepartmentName { get; set; }
}
