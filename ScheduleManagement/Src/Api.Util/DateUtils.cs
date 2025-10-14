using System.Globalization;
using ScheduleManagement.Api.Entity;

namespace ScheduleManagement.Api.Util;

public static class DateUtils
{
	public static string FormatTime(ScheduleSubject subject)
	{
		return $@"{subject.StartTime:hh\:mm} - {subject.EndTime:hh\:mm}";
	}

	public static string ConvertScheduleOccur(ScheduleSubject scheduleSubject)
	{
		return string.Join(", ", scheduleSubject.WeekScheduleOccurs
			.OrderBy(x => x.OccurDate)
			.Select(o => o.OccurDate.ToString("dd.MM")));
	}

	public static DateTime FirstDateOfWeekBasedWeekNumber(int year, int weekOfYear)
	{
		var jan1 = new DateTime(year, 1, 1);
		var daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

		var firstThursday = jan1.AddDays(daysOffset);

		var cal = CultureInfo.CurrentCulture.Calendar;
		var firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

		var weekNum = weekOfYear;
		if (firstWeek == 1) weekNum -= 1;
		var result = firstThursday.AddDays(weekNum * 7);
		return result.AddDays(-3);
	}

	public static DateTime FindDayBasedDayIdAndWeekNumber(int year, int weekNumber, int dayOfWeek)
	{
		var jan1 = new DateTime(year, 1, 1);
		var daysOffset = DayOfWeek.Tuesday - jan1.DayOfWeek;

		var firstMonday = jan1.AddDays(daysOffset);

		var cal = CultureInfo.CurrentCulture.Calendar;
		var firstWeek = cal.GetWeekOfYear(jan1, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

		var weekNum = weekNumber;
		if (firstWeek <= 1) weekNum -= 1;
		return firstMonday.AddDays(weekNum * 7 + dayOfWeek - 2);
	}

	public static (int pxFromTop, int pxHegith) ComputedPositionFromTopAndHeight(TimeSpan hourStart, TimeSpan hourEnd,
		int blockHeight = 96)
	{
		var heightOf5Minutes = blockHeight / (60 / 5);

		var allMinutesFromTop = (int)(hourStart - TimeSpan.Parse("07:00")).TotalMinutes;
		var pxFromTop = heightOf5Minutes * allMinutesFromTop / 5;

		var fullLength = (int)(hourEnd - hourStart).TotalMinutes / 5;
		var pxHegith = heightOf5Minutes * fullLength;

		return (pxFromTop, pxHegith);
	}
}
