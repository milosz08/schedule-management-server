using System.IO;
using System.Net;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Hosting;

using asp_net_po_schedule_management_server.Utils;


namespace asp_net_po_schedule_management_server.Ssh.SmtpEmailService
{
    public class SmtpEmailServiceImplementation : ISmtpEmailService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        
        // skrót do folderu z szablonami wiadomości email
        private const string _templatePath = @"EmailTemplates/{0}.html";

        private const string _resetPasswordTempl = "ResetPassword";
        private const string _addUserPassUserTempl = "NewUserToUser";
        
        //--------------------------------------------------------------------------------------------------------------
        
        public SmtpEmailServiceImplementation(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        //--------------------------------------------------------------------------------------------------------------
        
        #region Emails senders

        /// <summary>
        /// Metoda odpowiedzialna za wysyłanie adresu email z kodem w celu zresetowania hasła.
        /// </summary>
        /// <param name="userEmailOptions">parametry przesyłane do ciała wiadomości email</param>
        public async Task SendResetPassword(UserEmailOptions userEmailOptions)
        {
            await SendEmailTemplate(userEmailOptions, _resetPasswordTempl, "Resetowanie hasła: {{userName}}");
        }

        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Metoda odpowiedzialna za wysyłanie wiadomości email do nowo dodanego użytkownika z podstawowymi informacjami
        /// o systemie oraz nowo wygenerowanymi hasłami i kluczami dostępu.
        /// </summary>
        /// <param name="userEmailOptions">parametry przesyłane do ciała wiadomości email</param>
        public async Task SendCreatedUserAuthUser(UserEmailOptions userEmailOptions)
        {
            await SendEmailTemplate(userEmailOptions, _addUserPassUserTempl, "Witamy w Systemie Zarządzania Planem");
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Metoda szkielet odpowiedzialna za specyzowanie wiadomości email i wypełnianie jej treścią.
        /// </summary>
        /// <param name="userEmailOptions">parametry przesyłane do ciała wiadomości email</param>
        /// <param name="template">szablon emailu HTML</param>
        /// <param name="title">tytuł emailu</param>
        private async Task SendEmailTemplate(UserEmailOptions userEmailOptions, string template, string title)
        {
            userEmailOptions.Subject = CreatePlaceholders(title, userEmailOptions.Placeholders);
            userEmailOptions.Body = CreatePlaceholders(GetEmailBody(template), userEmailOptions.Placeholders);
            await SendEmail(userEmailOptions);
        }
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        // metoda odpowiadająca za konfigurację wysyłanego adresu email (możliwość wysyłki do większej
        // ilości użytkowników). Wartości przekazywane poprzez obiekt userEmailOptions.
        private async Task SendEmail(UserEmailOptions userEmailOptions)
        {
            MailMessage mail = new MailMessage()
            {
                Subject = userEmailOptions.Subject,
                Body = userEmailOptions.Body,
                From = new MailAddress(GlobalConfigurer.StmpSenderAddress, GlobalConfigurer.StmpSenderDisplayName),
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.Default,
                IsBodyHtml = GlobalConfigurer.IsBodyHTML,
            };

            // dodawanie wiadomości do wszystkich użytkowników
            foreach (var user in userEmailOptions.ToEmails) {
                mail.To.Add(user);
            }

            // autentykacja z serwerem STMP
            SmtpClient smtpClient = new SmtpClient()
            {
                Host = GlobalConfigurer.StmpHost,
                Port = GlobalConfigurer.StmpPort,
                EnableSsl = GlobalConfigurer.EnableSSL,
                UseDefaultCredentials = GlobalConfigurer.UseDefaultCredentials,
                Credentials = new NetworkCredential(GlobalConfigurer.StmpUsername, GlobalConfigurer.StmpPassword),
            };

            InsertEbededImage(mail, "main-logo-dark.jpg"); // dodanie zakotwiczonego obrazka
            
            // wysyłka wiadomości email
            mail.BodyEncoding = Encoding.Default;
            await smtpClient.SendMailAsync(mail);
        }

        //--------------------------------------------------------------------------------------------------------------
        
        // metoda odpowiedzialna za dołączanie szablonu HTML adresu email do wiadomości
        private string GetEmailBody(string templateName)
        {
            return File.ReadAllText(string.Format(_templatePath, templateName + "Template"));
        }

        //--------------------------------------------------------------------------------------------------------------
        
        // metoda zamieniająca wszystkie literały szablonowe na podstawione ciągi znaków (na podstawie
        // kontenera typu hash-mapa)
        private string CreatePlaceholders(string text, List<KeyValuePair<string, string>> keyValuePairs)
        {
            if (!string.IsNullOrEmpty(text) && keyValuePairs != null) {
                foreach (KeyValuePair<string, string> placeholder in keyValuePairs) {
                    text = text.Replace(placeholder.Key, placeholder.Value);
                }
            }
            return text;
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        // metoda dodająca zakotwiczony obrazek (logo aplikacji) do kontentu wiadomości email (obrazek
        // pozyskiwany z głównego katalogu wwwroot aplikacji)
        private void InsertEbededImage(MailMessage mailMessage, string imagePath)
        {
            string logoPath = Path.Combine(_hostingEnvironment.WebRootPath, "cdn/images", imagePath);
            AlternateView altView = AlternateView
                .CreateAlternateViewFromString(mailMessage.Body, null, MediaTypeNames.Text.Html);

            LinkedResource imageLogo = new LinkedResource(logoPath, MediaTypeNames.Image.Jpeg);
            imageLogo.ContentId = "ImageLogo";
            
            altView.LinkedResources.Add(imageLogo);
            mailMessage.AlternateViews.Add(altView);
        }
    }
}