using System;
using System.Text;


namespace asp_net_po_schedule_management_server.Utils
{
    public static class ApplicationUtils
    {
        private static readonly string RANDOM_CHARS = "abcdefghijklmnoprstquvwxyzABCDEFGHIJKLMNOPRSTQUWXYZ0123456789";
        private static readonly string RANDOM_NUMBERS = "0123456789";
        private static readonly string SPECIAL_CHARS = "@#%&$";
        private static readonly Random _random = new Random();
        
        // generowanie hasza słownikowego (głównie na potrzeby frontu)
        public static string DictionaryHashGenerator(int hashSize = 20)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashSize; i++) {
                int randomIndex = _random.Next(RANDOM_CHARS.Length);
                builder.Append(RANDOM_CHARS[randomIndex]);
            }
            return builder.ToString();
        }

        // generowanie randomowego zestawu cyfr (o długości na podstawie parametru)
        public static string RandomNumberGenerator(int randomSize = 3)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < randomSize; i++) {
                int randomIndex = _random.Next(RANDOM_NUMBERS.Length);
                builder.Append(RANDOM_NUMBERS[randomIndex]);
            }
            return builder.ToString();
        }

        // generowanie początkowego hasła dla użytkowników (możliwośc zmiany na własne)
        public static string GenerateUserFirstPassword()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(DictionaryHashGenerator(6));
            builder.Append(RandomNumberGenerator());
            builder.Append(SPECIAL_CHARS[_random.Next(SPECIAL_CHARS.Length)]);
            builder.Append(DictionaryHashGenerator(6));
            return builder.ToString();
        }
        
        // funkcja konwertująca obiekt Date na chwilę czasową
        public static String GetTimestamp(DateTime date)
        {
            return date.ToString("yyyyMMddHHmmssffff");
        }
    }
}