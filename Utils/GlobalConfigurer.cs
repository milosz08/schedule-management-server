using System;


namespace asp_net_po_schedule_management_server.Utils
{
    public class GlobalConfigurer
    {
        // obsługa JWT
        public static string JwtKey { get; set; }
        public static TimeSpan JwtExpiredTimestamp { get; set; }
        
        //--------------------------------------------------------------------------------------------------------------
        
        // obsługa SSH
        public static string SshServer { get; set; }
        public static string SshUsername { get; set; }
        public static string SshPassword { get; set; }
        public static string SshPasswordFieldName { get; set; }
        
        //--------------------------------------------------------------------------------------------------------------
        
        // obsługa email
        public static string StmpSenderAddress { get; set; }
        public static string StmpSenderDisplayName { get; set; }
        public static string StmpUsername { get; set; }
        public static string StmpPassword { get; set; }
        public static string StmpHost { get; set; }
        public static int StmpPort { get; set; }
        public static bool EnableSSL { get; set; }
        public static bool UseDefaultCredentials { get; set; }
        public static bool IsBodyHTML { get; set; }
        
        //--------------------------------------------------------------------------------------------------------------
        
        // inne
        public static string UserEmailDomain { get; set; }
        public static string DbDriverVersion { get; set; }
        public static byte UserEmailMaxSizeMb { get; set; }
        public static TimeSpan OptExpired { get; set; }
        public static InitialUserAccount InitialCredentials { get; set; }
    }

    public class InitialUserAccount
    {
        public string AccountName { get; set; }
        public string AccountSurname { get; set; }
        public string AccountPassword { get; set; }
    }
}