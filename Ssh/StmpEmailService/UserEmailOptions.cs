using System.Collections.Generic;

namespace asp_net_po_schedule_management_server.Ssh.StmpEmailService
{
    public class UserEmailOptions
    {
        // lista adresów email do jakich system wyśle emaile
        public List<string> ToEmails { get; set; }
        
        // tytuł emailu
        public string Subject { get; set; }
        
        // ciało emailu (szablon HTML)
        public string Body { get; set; }
        
        // literały szablonowe zastępowane przez zmienne
        public List<KeyValuePair<string, string>> Placeholders { get; set; }
    }
}