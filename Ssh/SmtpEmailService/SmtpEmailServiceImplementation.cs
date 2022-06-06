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
    /// <summary>
    /// Klasa odpowiedzialna za wysyłanie wiadomości email do klientów poprzez protokół SMTP
    /// </summary>
    public class SmtpEmailServiceImplementation : ISmtpEmailService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        
        // skrót do folderu z szablonami wiadomości email
        private const string _templatePath = @"EmailTemplates/{0}.html";

        private const string _resetPasswordTempl = "ResetPassword";
        private const string _addUserPassUserTempl = "NewUserToUser";
        private const string _contactFormMessage = "ContactFormMessageCopy";
        
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
        /// Metoda odpowiedzialna za wysyłanie wiadomości email do użytkownika oraz administratorów/moderatorów po
        /// wysłaniu przez użytkownika wiadomości w formularzu.
        /// </summary>
        /// <param name="userEmailOptions">parametry przesyłane do ciała wiadomości email</param>
        /// <param name="issueIdentified">identyfikator wiadomości</param>
        public async Task SendNewContactMessage(UserEmailOptions userEmailOptions, string issueIdentified)
        {
            await SendEmailTemplate(userEmailOptions, _contactFormMessage, $"Zgłoszenie nr {issueIdentified}");
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
        
        /// <summary>
        /// Metoda odpowiadająca za konfigurację wysyłanego adresu email (możliwość wysyłki do większej
        //  ilości użytkowników). Wartości przekazywane poprzez obiekt userEmailOptions.
        /// </summary>
        /// <param name="userEmailOptions">parametry przesyłane do ciała wiadomości email</param>
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
        
        /// <summary>
        /// Metoda odpowiedzialna za dołączanie szablonu HTML adresu email do wiadomości.
        /// </summary>
        /// <param name="templateName">nazwa szablonu email</param>
        /// <returns>czysty tekst HTML z odczytanego szablonu</returns>
        private string GetEmailBody(string templateName)
        {
            return File.ReadAllText(string.Format(_templatePath, templateName + "Template"));
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Metoda zamieniająca wszystkie literały szablonowe na podstawione ciągi znaków (na podstawie
        //  kontenera typu hash-mapa).
        /// </summary>
        /// <param name="text">początkowy rezultat nadpisywany filtrowaniem literałów</param>
        /// <param name="keyValuePairs">literały wraz z ich wartościami</param>
        /// <returns>przefiltrowany dokument HTML ze wstawionymi danymi</returns>
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
        
        /// <summary>
        /// Metoda dodająca zakotwiczony obrazek (logo aplikacji) do kontentu wiadomości email (obrazek
        //  pozyskiwany z głównego katalogu wwwroot aplikacji).
        /// </summary>
        /// <param name="mailMessage">wiadomość email</param>
        /// <param name="imagePath">ścieżka względna do obrazka</param>
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