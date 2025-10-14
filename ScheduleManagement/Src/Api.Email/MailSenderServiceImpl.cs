using System.Globalization;
using System.Net;
using System.Net.Mail;
using FluentEmail.Core.Models;
using FluentEmail.Liquid;
using FluentEmail.Smtp;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using ScheduleManagement.Api.Config;
using ScheduleManagement.Api.Exception;
using Attachment = FluentEmail.Core.Models.Attachment;

namespace ScheduleManagement.Api.Email;

public class MailSenderServiceImpl(ILogger<MailSenderServiceImpl> logger, IWebHostEnvironment environment)
	: IMailSenderService
{
	public async Task SendEmail<T>(UserEmailOptions<T> userEmailOptions, string templateName)
		where T : AbstractMailViewModel
	{
		var templateFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates");
		try
		{
			var fileProvider = new PhysicalFileProvider(templateFilesPath);
			var options = new LiquidRendererOptions
			{
				FileProvider = fileProvider
			};
			var smtpClient = new SmtpClient
			{
				Host = ApiConfig.Smtp.Host,
				Port = int.Parse(ApiConfig.Smtp.Port),
				EnableSsl = ApiConfig.Smtp.EnableSsl,
				Credentials = new NetworkCredential(ApiConfig.Smtp.Username, ApiConfig.Smtp.Password)
			};
			FluentEmail.Core.Email.DefaultSender = new SmtpSender(smtpClient);
			FluentEmail.Core.Email.DefaultRenderer = new LiquidRenderer(Options.Create(options));

			userEmailOptions.DataModel.CurrentYear = DateTime.Now.Year.ToString();
			userEmailOptions.DataModel.CurrentDate = DateTime.Now.ToString(CultureInfo.DefaultThreadCurrentCulture);
			userEmailOptions.DataModel.AboutProjectUrl = ApiConfig.AboutUrl;
			userEmailOptions.DataModel.ClientOrigin = ApiConfig.ClientOrigin;

			var templateRawContent =
				await File.ReadAllTextAsync(Path.Combine("EmailTemplates", $"{templateName}.liquid"));

			List<Address> addresses = [];
			foreach (var user in userEmailOptions.ToEmails) addresses.Add(new Address(user));
			var fluentEmail = FluentEmail.Core.Email
				.From($"noreply@{ApiConfig.Smtp.EmailDomain}", "System Zarządzania Planem")
				.ReplyTo($"info@{ApiConfig.Smtp.EmailDomain}", "System Zarządzania Planem")
				.To(addresses)
				.Subject($"Schedule Management Server | {userEmailOptions.Subject}")
				.UsingTemplate(templateRawContent, userEmailOptions.DataModel);

			var logoPath = Path.Combine(environment.WebRootPath, "images", "main-logo-dark.jpg");
			var imageBase64 = Convert.FromBase64String(Convert.ToBase64String(await File.ReadAllBytesAsync(logoPath)));

			using var logoStream = new MemoryStream(imageBase64);

			fluentEmail.Attach(new Attachment
			{
				IsInline = true,
				Filename = "ImageLogo",
				ContentId = "ImageLogo",
				Data = logoStream,
				ContentType = "image/jpg"
			});
			var sendResponse = await fluentEmail.SendAsync();
			if (!sendResponse.Successful) throw new System.Exception(string.Join(",", sendResponse.ErrorMessages));
		}
		catch (System.Exception ex)
		{
			logger.LogError("Unable to send email message. Cause: {}", ex.Message);
			throw new RestApiException("Nieudane wysłanie wiadomości email. Spróbuj ponownie później.",
				HttpStatusCode.InternalServerError);
		}
	}
}
