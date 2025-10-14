namespace ScheduleManagement.Api.Email;

public interface IMailboxProxyService
{
	void AddNewEmailAccount(string emailAddress, string emailPassword);

	void DeleteEmailAccount(string emailAddress);
}
