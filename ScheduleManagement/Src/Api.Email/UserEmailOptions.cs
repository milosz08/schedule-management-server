namespace ScheduleManagement.Api.Email;

public class UserEmailOptions<T> where T : AbstractMailViewModel
{
	public List<string> ToEmails { get; set; }
	public string Subject { get; set; }
	public T DataModel { get; set; }
}
