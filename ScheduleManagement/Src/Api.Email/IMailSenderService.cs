namespace ScheduleManagement.Api.Email;

public interface IMailSenderService
{
	Task SendEmail<T>(UserEmailOptions<T> userEmailOptions, string templateName) where T : AbstractMailViewModel;
}
