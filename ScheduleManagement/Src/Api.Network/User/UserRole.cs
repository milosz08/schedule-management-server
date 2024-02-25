using ScheduleManagement.Api.Entity;

namespace ScheduleManagement.Api.Network.User;

public static class UserRole
{
	public const string Student = "student";
	public const string Teacher = "nauczyciel";
	public const string Editor = "edytor";
	public const string Administrator = "administrator";

	private static bool CheckRole(string role, Person person)
	{
		return role.Equals(person.Role.Name);
	}

	public static bool IsAdministrator(Person person)
	{
		return CheckRole(Administrator, person);
	}
}
