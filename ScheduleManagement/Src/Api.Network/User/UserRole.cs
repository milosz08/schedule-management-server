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

	public static bool IsAdministrator(string role)
	{
		return Administrator.Equals(role);
	}

	public static bool IsStudent(Person person)
	{
		return CheckRole(Student, person);
	}

	public static bool IsStudent(string role)
	{
		return Student.Equals(role);
	}

	public static bool IsTeacher(Person person)
	{
		return CheckRole(Teacher, person);
	}

	public static bool IsTeacher(string role)
	{
		return Teacher.Equals(role);
	}

	public static bool IsEditor(Person person)
	{
		return CheckRole(Editor, person);
	}

	public static bool IsEditor(string role)
	{
		return Editor.Equals(role);
	}
}
