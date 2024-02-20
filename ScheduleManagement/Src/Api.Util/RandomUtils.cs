using System.Text;

namespace ScheduleManagement.Api.Util;

public static class RandomUtils
{
	private const string RandomChars = "abcdefghijklmnoprstquvwxyzABCDEFGHIJKLMNOPRSTQUWXYZ0123456789";
	private const string RandomNumbers = "0123456789";

	private static readonly Random Random = new();

	public static string RandomStringGenerator(int hashSize = 20)
	{
		var builder = new StringBuilder();
		for (var i = 0; i < hashSize; i++)
		{
			var randomIndex = Random.Next(RandomChars.Length);
			builder.Append(RandomChars[randomIndex]);
		}
		return builder.ToString();
	}

	public static string RandomNumberGenerator(int randomSize = 3)
	{
		var builder = new StringBuilder();
		for (var i = 0; i < randomSize; i++)
		{
			var randomIndex = Random.Next(RandomNumbers.Length);
			builder.Append(RandomNumbers[randomIndex]);
		}
		return builder.ToString();
	}

	public static string GenerateUserFirstPassword()
	{
		var builder = new StringBuilder();
		builder.Append(RandomStringGenerator(6));
		builder.Append(RandomNumberGenerator());
		builder.Append(RandomStringGenerator(6));
		return builder.ToString();
	}
}