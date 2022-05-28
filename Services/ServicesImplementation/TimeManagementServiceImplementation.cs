using System;
using System.Collections.Generic;


namespace asp_net_po_schedule_management_server.Services.ServicesImplementation
{
    public sealed class TimeManagementServiceImplementation : ITimeManagementService
    {
        #region Return all weeks indicators in current study year
        
        /// <summary>
        /// Metoda zwracająca listę z tygodniami (pierwszy i ostatni dzień, nr tygodnia oraz rok) na podstawie
        /// wyliczonego aktualnego roku akademickiego.
        /// </summary>
        /// <returns>lista tygodni</returns>
        public List<string> GetAllWeeksNameWithWeekNumberInCurrentYear()
        {
            List<string> allDates = new List<string>();
            int startYear, endYear;
            
            // jeśli jest to 2 semestr, ustaw rok poprzedni, jeśli nie (semestr 1) ustaw rok kolejny
            if (DateTime.Now.Month > 1 && DateTime.Now.Month < 10) {
                startYear = DateTime.Now.Year - 1;
                endYear = DateTime.Now.Year;
            } else {
                startYear = DateTime.Now.Year;
                endYear = DateTime.Now.Year + 1;
            }
            
            DateTime start = new DateTime(startYear, 10, 1);
            DateTime end = new DateTime(endYear, 9, 30);

            double daysBefore = (start - new DateTime(start.Year, 1, 1)).TotalDays;

            int weekNumber = (int) Math.Ceiling(daysBefore / 7);
            int currentYear = start.Year;

            // przejście przez wszystkie dni tygodnia z podanego zakresu lat
            for (DateTime dt = start; dt <= end; dt = dt.AddDays(7))
            {
                if (dt.Year > currentYear) {
                    currentYear = dt.Year;
                    weekNumber = 1;
                }

                // obliczanie pierszego dnia tygodnia na podstawie numeru tygodnia i roku
                DateTime firstDate = new DateTime(dt.Year, 1, 4);
                while (firstDate.DayOfWeek != DayOfWeek.Monday) {
                    firstDate = firstDate.AddDays(-1);
                }

                DateTime firstDay = firstDate.AddDays((weekNumber - 1) * 7);
                DateTime lastDay = firstDay.AddDays(6);

                string firstDayFormat = firstDay.Day < 10 ? $"0{firstDay.Day}" : firstDay.Day.ToString();
                string firstMothFormat = firstDay.Month < 10 ? $"0{firstDay.Month}" : firstDay.Month.ToString();
                
                string lastDayFormat = lastDay.Day < 10 ? $"0{lastDay.Day}" : lastDay.Day.ToString();
                string lastMothFormat = lastDay.Month < 10 ? $"0{lastDay.Month}" : lastDay.Month.ToString();
                
                allDates.Add($"{firstDayFormat}.{firstMothFormat} - {lastDayFormat}.{lastMothFormat} " +
                             $"({dt.Year}, {weekNumber})");

                ++weekNumber;
            }

            return allDates;
        }

        #endregion
    }
}