using System.Collections.Generic;

namespace asp_net_po_schedule_management_server.Ssh.SmtpEmailService
{
    public class UserEmailOptions
    {
        /// <summary>
        /// Lista adresów email do jakich system wyśle emaile.
        /// </summary>
        public List<string> ToEmails { get; set; }
        
        /// <summary>
        /// Tytuł emailu.
        /// </summary>
        public string Subject { get; set; }
        
        /// <summary>
        /// Ciało emailu (szablon HTML).
        /// </summary>
        public string Body { get; set; }
        
        /// <summary>
        /// Literały szablonowe zastępowane przez zmienne.
        /// </summary>
        public List<KeyValuePair<string, string>> Placeholders { get; set; }
    }
}