namespace ScheduleManagement.Api.Network.LastOpenedSchedules;

public sealed class LastOpenedScheduleData
{
	public long Id { get; set; }
	public string Name { get; set; }
	public long DeptId { get; set; }
	public long SpecId { get; set; }
	public long GroupId { get; set; }
}

public sealed class LastOpenedScheduleRequestDto
{
	public long DeptId { get; set; }
	public long SpecId { get; set; }
	public long GroupId { get; set; }
}

public sealed class LastOpenedScheduleResponseDto
{
	public string DeptId { get; set; }
	public string SpecId { get; set; }
	public string GroupId { get; set; }
}
