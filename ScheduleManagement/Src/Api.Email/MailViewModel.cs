namespace ScheduleManagement.Api.Email;

public class ContactFormMessageCopyViewModel : AbstractMailViewModel
{
	public string MessageId { get; set; }
	public string IssueType { get; set; }
	public string DepartmentName { get; set; }
	public string GroupNames { get; set; }
	public string Description { get; set; }
}

public class NewUserToUserViewModel : AbstractMailViewModel
{
	public string Login { get; set; }
	public string Password { get; set; }
	public string UserRole { get; set; }
}

public class ResetPasswordViewModel : AbstractMailViewModel
{
	public int ExpiredInMinutes { get; set; }
	public string Token { get; set; }
}
