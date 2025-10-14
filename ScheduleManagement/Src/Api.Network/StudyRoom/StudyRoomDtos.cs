using System.ComponentModel.DataAnnotations;

namespace ScheduleManagement.Api.Network.StudyRoom;

public sealed class StudyRoomRequestDto
{
	[Required(ErrorMessage = "Pole nazwy (aliasu) sali nie może być puste")]
	public string Name { get; set; }

	public string Description { get; set; }

	[Required(ErrorMessage = "Pole nazwy wydziału nie może być puste")]
	public string DepartmentName { get; set; }

	[Required(ErrorMessage = "Pole nazwy katedry nie może być puste")]
	public string CathedralName { get; set; }

	[Required(ErrorMessage = "Pole pojemności sali nie może być puste")]
	public int Capacity { get; set; }

	[Required(ErrorMessage = "Pole typu sali nie może być puste")]
	public string RoomTypeName { get; set; }
}

public sealed class StudyRoomResponseDto
{
	public string Name { get; set; }
	public string Description { get; set; }
	public int Capacity { get; set; }
	public string DepartmentFullName { get; set; }
	public string CathedralFullName { get; set; }
	public string RoomTypeFullName { get; set; }
}

public sealed class StudyRoomQueryResponseDto
{
	public long Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public int Capacity { get; set; }
	public string DepartmentName { get; set; }
	public string DepartmentAlias { get; set; }
	public string CathedralName { get; set; }
	public string CathedralAlias { get; set; }
	public string DeptWithCathAlias { get; set; }
	public string RoomTypeName { get; set; }
	public string RoomTypeAlias { get; set; }
}

public sealed class StudyRoomEditResDto
{
	public string Name { get; set; }
	public string Description { get; set; }
	public string DepartmentName { get; set; }
	public string CathedralName { get; set; }
	public int Capacity { get; set; }
	public string RoomTypeName { get; set; }
}
