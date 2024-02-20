namespace ScheduleManagement.Api.Dto;

public class NameIdElementDto(long id, string name)
{
	private long Id { get; set; } = id;
	public string Name { get; init; } = name;
}
