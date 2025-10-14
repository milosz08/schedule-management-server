using ScheduleManagement.Api.Config;
using ScheduleManagement.Api.Ssh;

namespace ScheduleManagement.Api.Email;

public class MailboxProxyServiceImpl(
	ISshInterceptor sshInterceptor,
	ILogger<MailboxProxyServiceImpl> logger)
	: IMailboxProxyService
{
	public void AddNewEmailAccount(string emailAddress, string emailPassword)
	{
		var capacity = ApiConfig.MailboxManagerCommand?.SetCapacity;
		sshInterceptor.ExecuteCommand(
			string.Format(ApiConfig.MailboxManagerCommand?.Create!, emailAddress, emailPassword));
		sshInterceptor.ExecuteCommand(string.Format(capacity!, emailAddress,
			ApiConfig.EmailCapacityMb));
		logger.LogInformation("Successfully created email account: {} with capacity: {}", emailAddress, capacity);
	}

	public void UpdateEmailPassword(string emailAddress, string newEmailPassword)
	{
		sshInterceptor.ExecuteCommand(string.Format(ApiConfig.MailboxManagerCommand?.UpdatePassword!, emailAddress,
			newEmailPassword));
		logger.LogInformation("Successfully updated password for email: {}", emailAddress);
	}

	public void DeleteEmailAccount(string emailAddress)
	{
		sshInterceptor.ExecuteCommand(string.Format(ApiConfig.MailboxManagerCommand?.Delete!, emailAddress));
		logger.LogInformation("Successfully removed email account with email: {}", emailAddress);
	}
}
