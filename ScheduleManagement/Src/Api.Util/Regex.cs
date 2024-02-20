namespace ScheduleManagement.Api.Util;

public static class Regex
{
	public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$";
}
