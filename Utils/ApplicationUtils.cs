/*
 * Copyright (c) 2022 by MILOSZ GILGA <https://miloszgilga.pl> <https://github.com/Milosz08>
 * Silesian University of Technology | Politechnika Śląska
 *
 * File name | Nazwa pliku: ApplicationUtils.cs
 * Project name | Nazwa Projektu: asp-net-po-schedule-management-server
 *
 * Klient | Client: <https://github.com/Milosz08/Angular_PO_Schedule_Management_Client>
 * Serwer | Server: <https://github.com/Milosz08/ASP.NET_PO_Schedule_Management_Server>
 *
 * RestAPI for the Angular application to manage schedule for sample university. Written with the ASP.NET Core
 * and Entity Framework with mySQL database. Project for the teaching course "Objected Oriented Programming".
 *
 * RestAPI dla aplikacji Angular do zarządzania planem zajęć przykładowej uczelni wyższej. Napisane w oparciu o
 * ASP.NET Core oraz Entity Framework z bazą danych mySQL. Projekt wykonany na zajęcia "Programowanie Obiektowe".
 */

using System;

using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;

using Microsoft.AspNetCore.Hosting;

using asp_net_po_schedule_management_server.Entities;
using asp_net_po_schedule_management_server.Exceptions;


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
        
        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Metoda umożliwiająca deserializację pliku JSON na sparametryzowany obiekt generyczny.
        /// </summary>
        /// <param name="fileName">nazwa pliku wejściowego</param>
        /// <param name="hostEnvironment">ścieżka środowiskowa</param>
        /// <typeparam name="T">parametr generyczny obiektu na który ma zostać zrobiona deserializacja</typeparam>
        /// <returns>obiekt z deserializowanymi danymi</returns>
        /// <exception cref="BasicServerException">nieprawidłowy plik JSON/brak pliku</exception>
        public static List<T> ConvertJsonToList<T>(string fileName, IWebHostEnvironment hostEnvironment)
        {
            string roomTypesPath = Path.Combine(hostEnvironment.WebRootPath, "cdn/mocked", fileName);
            string jsonString = File.ReadAllText(roomTypesPath);
            List<T> deserialisedArray;
            try {
                deserialisedArray = JsonSerializer.Deserialize<List<T>>(jsonString);
            } catch (JsonException ex) {
                throw new BasicServerException("Nieprawidłowy format pliku json! Stacktrace: " + ex.Message,
                    HttpStatusCode.InternalServerError);
            }
            return deserialisedArray;
        }
        
        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Metoda tworząca alias przedmiotu.
        /// </summary>
        /// <param name="subjectName">pełna nazwa przedmiotu</param>
        /// <returns>wynikowy alias w postaci "2 pierwsze litery/KIER/WYDZ"</returns>
        public static string CreateSubjectAlias(string subjectName)
        {
            string[] namePieces = subjectName.Split(" ");
            int iterator = 0;
            StringBuilder builder = new StringBuilder();
            foreach (string piece in namePieces) {
                if (iterator == 0) {
                    builder.Append(piece.Substring(0, 1).ToUpper());
                } else {
                    builder.Append(piece.Substring(0, 1).ToLower());
                }
                iterator++;
            }
            return builder.ToString();
        }
        
        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Metoda formatująca czas odbywania przedmitu z typów TimeSpan na string.
        /// </summary>
        /// <param name="subject">przedmiot z planu zajęć</param>
        /// <returns>przeformatowany czas początkowy i końcowy w formie HH:mm - HH:mm</returns>
        public static string FormatTime(ScheduleSubject subject)
        {
            return $"{subject.StartTime.ToString("hh\\:mm")} - {subject.EndTime.ToString("hh\\:mm")}";
        }
        
        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Metoda konwertująca, sortująca i formatująca występowanie danego przedmiotu w planie zajęć grupy.
        /// </summary>
        /// <param name="scheduleSubject">przedmiot z planu zajęć</param>
        /// <returns>posortowane daty występowania przedmiotu w formie stringa</returns>
        public static string ConvertScheduleOccur(ScheduleSubject scheduleSubject)
        {
            return string.Join(", ", scheduleSubject.WeekScheduleOccurs
                .OrderBy(x => x.OccurDate)
                .Select(o => o.OccurDate.ToString("dd.MM")));
        }
        
        //--------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Metoda generująca alias przedmiotu (na podtrzeby obiektu transferowego przekazywanego w planie zajęć).
        /// </summary>
        /// <param name="scheduleSubject">przedmiot z planu zajęć</param>
        /// <returns>alias przedmiotu</returns>
        public static string CreateSubjectAlias(ScheduleSubject scheduleSubject)
        {
            return scheduleSubject.StudySubject.Alias.Substring(0, scheduleSubject.StudySubject.Alias.IndexOf("/",
                StringComparison.OrdinalIgnoreCase)) + ", " + scheduleSubject.ScheduleSubjectType.Alias;
        }
    }
}