using ScheduleManagement.Api.Entity;

namespace ScheduleManagement.Api.Dto;

public class UserCredentialsHeaderDto
{
	public string Login { get; set; }
	public string Username { get; set; }
	public string Password { get; set; }
	public Person Person { get; set; }
}
