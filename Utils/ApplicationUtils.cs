using System;
using System.Text;


namespace asp_net_po_schedule_management_server.Utils
{
    /// <summary>
    /// Klasa przechowująca metody pomocnicze w serwisach oraz innych klasach aplikacji.
    /// </summary>
    public static class ApplicationUtils
    {
        private static readonly string RANDOM_CHARS = "abcdefghijklmnoprstquvwxyzABCDEFGHIJKLMNOPRSTQUWXYZ0123456789";
        private static readonly string RANDOM_NUMBERS = "0123456789";
        private static readonly Random _random = new Random();
        
        public static readonly int[] _allowedPageSizes = new[] { 5, 10, 15, 30 };
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Generowanie hasza słownikowego (głównie na potrzeby front-endu).
        /// </summary>
        /// <param name="hashSize">rozmiar (ilość) hasza słownikowego</param>
        /// <returns>stworzony hasz słownikowy</returns>
        public static string DictionaryHashGenerator(int hashSize = 20)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashSize; i++) {
                int randomIndex = _random.Next(RANDOM_CHARS.Length);
                builder.Append(RANDOM_CHARS[randomIndex]);
            }
            return builder.ToString();
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Generowanie randomowego zestawu cyfr (o długości na podstawie parametru).
        /// </summary>
        /// <param name="randomSize">rozmiar (ilość) zestawu cyfr</param>
        /// <returns>stworzony string z pseudolosowych znaków</returns>
        public static string RandomNumberGenerator(int randomSize = 3)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < randomSize; i++) {
                int randomIndex = _random.Next(RANDOM_NUMBERS.Length);
                builder.Append(RANDOM_NUMBERS[randomIndex]);
            }
            return builder.ToString();
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Generowanie początkowego hasła dla użytkowników (możliwośc zmiany na własne).
        /// </summary>
        /// <returns>wygenerowane hasło</returns>
        public static string GenerateUserFirstPassword()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(DictionaryHashGenerator(6));
            builder.Append(RandomNumberGenerator());
            builder.Append(DictionaryHashGenerator(6));
            return builder.ToString();
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Metoda konwertująca obiekt Date na chwilę czasową.
        /// </summary>
        /// <param name="date">obiekt typu Date</param>
        /// <returns>data w formie stringa</returns>
        public static String GetTimestamp(DateTime date)
        {
            return date.ToString("yyyyMMddHHmmssffff");
        }
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Metoda zmieniajca wszystkie znaki diakrytyczne języka polskiego na odpowiedniki w języku angielskim.
        /// </summary>
        /// <param name="text">ciąg pierwotny</param>
        /// <returns>ciąg wynikowy bez znaków diakrytycznych</returns>
        public static string RemoveAccents(string text)
        {
            string[] diacretics = { "ą", "ć", "ę", "ł", "ń", "ó", "ś", "ź", "ż" };
            string[] normalLetters = { "a", "c", "e", "l", "n", "o", "s", "z", "z" };
            string output = text;
            for (int i = 0; i < diacretics.Length; i++) {
                output = output.Replace(diacretics[i], normalLetters[i]);
            }
            return output;
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Ustawianie pierwszej litery słowa na wielką literę (capitalization).
        /// </summary>
        /// <param name="text">ciąg do przerobienia</param>
        /// <returns>ciąg wynikowy</returns>
        public static string CapitalisedLetter(string text)
        {
            return char.ToUpper(text[0]) + text.Substring(1);
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Pobieranie i zwracanie dzisiejszej sformwatowanej daty (używane głównie w wiadomościach email).
        /// </summary>
        /// <returns>sformatowana data</returns>
        public static string GetCurrentUTCdateString()
        {
            return $"{DateTime.UtcNow.ToShortDateString()}, {DateTime.UtcNow.ToShortTimeString()}";
        }
    }
}