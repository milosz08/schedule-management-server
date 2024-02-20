using System.Text;
using ScheduleManagement.Api.Entity;

namespace ScheduleManagement.Api.Util;

public static class StringUtils
{
	public static string CapitalisedLetter(string text)
	{
		return char.ToUpper(text[0]) + text[1..];
	}

	public static string CreateSubjectAlias(string subjectName)
	{
		var namePieces = subjectName.Split(" ");
		var iterator = 0;
		var builder = new StringBuilder();
		foreach (var piece in namePieces)
		{
			builder.Append(iterator == 0 ? piece[..1].ToUpper() : piece[..1].ToLower());
			iterator++;
		}
		return builder.ToString();
	}

	public static string CreateSubjectAlias(ScheduleSubject scheduleSubject)
	{
		return scheduleSubject.StudySubject.Alias[..scheduleSubject.StudySubject.Alias.IndexOf("/",
			StringComparison.OrdinalIgnoreCase)] + ", " + scheduleSubject.ScheduleSubjectType.Alias;
	}

	public static string RemoveAccents(string text)
	{
		string[] diacretics = ["ą", "ć", "ę", "ł", "ń", "ó", "ś", "ź", "ż"];
		string[] normalLetters = ["a", "c", "e", "l", "n", "o", "s", "z", "z"];
		var output = text;
		for (var i = 0; i < diacretics.Length; i++)
		{
			output = output.Replace(diacretics[i], normalLetters[i]);
		}
		return output;
	}
}
