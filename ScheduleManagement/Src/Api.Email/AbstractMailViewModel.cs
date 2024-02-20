namespace ScheduleManagement.Api.Email;

public abstract class AbstractMailViewModel
{
	public string UserName { get; set; }
	public string CurrentDate { get; set; }
	public string CurrentYear { get; set; }
	public string AboutProjectUrl { get; set; }
	public string ClientOrigin { get; set; }
}
