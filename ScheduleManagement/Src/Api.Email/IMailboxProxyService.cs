namespace ScheduleManagement.Api.Email;

public interface IMailboxProxyService
{
	void AddNewEmailAccount(string emailAddress, string emailPassword);

	void UpdateEmailPassword(string emailAddress, string newEmailPassword);

	void DeleteEmailAccount(string emailAddress);
}