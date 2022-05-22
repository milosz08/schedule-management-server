using System.Collections.Generic;


namespace asp_net_po_schedule_management_server.Ssh.SmtpEmailService
{
    /// <summary>
    /// Klasa przechowująca parametry wysyłanej wiadomości email.
    /// </summary>
    public class UserEmailOptions
    {
        // Lista adresów email do jakich system wyśle emaile.
        public List<string> ToEmails { get; set; }
        
        // Tytuł emailu.
        public string Subject { get; set; }
        
        // Ciało emailu (szablon HTML).
        public string Body { get; set; }
        
        // Literały szablonowe zastępowane przez zmienne.
        public List<KeyValuePair<string, string>> Placeholders { get; set; }
    }
}