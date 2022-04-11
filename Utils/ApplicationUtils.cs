using System;
using System.Text;


namespace asp_net_po_schedule_management_server.Utils
{
    public static class ApplicationUtils
    {
        private static readonly string RANDOM_CHARS = "abcdefghijklmnoprstquvwxyzABCDEFGHIJKLMNOPRSTQUWXYZ0123456789";
        private static readonly Random _random = new Random();
        
        // generowanie hasza słownikowego (głównie na potrzeby frontu)
        public static string DictionaryHashGenerator()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < 20; i++) {
                int randomIndex = _random.Next(RANDOM_CHARS.Length);
                builder.Append(RANDOM_CHARS[randomIndex]);
            }
            return builder.ToString();
        }
        
        // funkcja konwertująca obiekt Date na chwilę czasową (liczba sekund)
        public static String GetTimestamp(DateTime date)
        {
            return date.ToString("yyyyMMddHHmmssffff");
        }
    }
}