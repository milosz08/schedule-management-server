namespace ScheduleManagement.Api.Network.TimeManagement;

public class TimeManagementServiceImpl : ITimeManagementService
{
	public List<string> GetAllStudyYearsFrom2020ToCurrent()
	{
		const int initialYear = 2020;
		var currentYear = DateTime.Now.Year;
		var allDates = new List<string>();
		for (var i = 0; i < currentYear - initialYear + 1; i++)
		{
			allDates.Add($"{initialYear + i}/{initialYear + 1 + i}");
		}
		return allDates;
	}

	public List<string> GetAllWeeksNameWithWeekNumberInCurrentYear(int startYear, int endYear)
	{
		var allDates = new List<string>();
		var start = new DateTime(startYear, 10, 1);
		var end = new DateTime(endYear, 9, 30);

		var daysBefore = (start - new DateTime(start.Year, 1, 1)).TotalDays;

		var weekNumber = (int)Math.Ceiling(daysBefore / 7);
		var currentYear = start.Year;

		for (var dt = start; dt <= end; dt = dt.AddDays(7))
		{
			if (dt.Year > currentYear)
			{
				currentYear = dt.Year;
				weekNumber = 1;
			}
			var firstDate = new DateTime(dt.Year, 1, 4);
			while (firstDate.DayOfWeek != DayOfWeek.Monday)
			{
				firstDate = firstDate.AddDays(-1);
			}
			var firstDay = firstDate.AddDays((weekNumber - 1) * 7);
			var lastDay = firstDay.AddDays(6);

			var firstDayFormat = firstDay.Day < 10 ? $"0{firstDay.Day}" : firstDay.Day.ToString();
			var firstMonthFormat = firstDay.Month < 10 ? $"0{firstDay.Month}" : firstDay.Month.ToString();

			var lastDayFormat = lastDay.Day < 10 ? $"0{lastDay.Day}" : lastDay.Day.ToString();
			var lastMothFormat = lastDay.Month < 10 ? $"0{lastDay.Month}" : lastDay.Month.ToString();

			allDates.Add($"{firstDayFormat}.{firstMonthFormat} - {lastDayFormat}.{lastMothFormat} " +
			             $"({dt.Year}, {weekNumber})");
			++weekNumber;
		}
		return allDates;
	}
}
