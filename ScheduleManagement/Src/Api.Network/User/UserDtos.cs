namespace ScheduleManagement.Api.Network.User;

public sealed class UserResponseDto
{
	public long Id { get; set; }
	public string NameWithSurname { get; set; }
	public string Login { get; set; }
	public string Role { get; set; }
	public bool IsRemovable { get; set; }
}

public sealed class DashboardDetailsResDto
{
	public DashboardElementsCount DashboardElementsCount;
	public DashboardUserTypesCount DashboardUserTypesCount;
	public string Email { get; set; }
	public string Shortcut { get; set; }
	public string City { get; set; }
	public string Nationality { get; set; }
	public string DepartmentFullName { get; set; }
	public string CathedralFullName { get; set; }
	public List<string> StudySpecializations { get; set; } = [];
	public List<string> StudySubjects { get; set; } = [];
}

public sealed class DashboardElementsCount
{
	public DashboardElementsCount(int dept, int cath, int room, int spec, int subj, int group)
	{
		DepartmentsCount = dept;
		CathedralsCount = cath;
		StudyRoomsCount = room;
		StudySpecializationsCount = spec;
		StudySubjectsCount = subj;
		StudyGroupsCount = group;
		AllElements = dept + cath + room + spec + subj + group;
	}

	public int DepartmentsCount { get; set; }
	public int CathedralsCount { get; set; }
	public int StudyRoomsCount { get; set; }
	public int StudySpecializationsCount { get; set; }
	public int StudySubjectsCount { get; set; }
	public int StudyGroupsCount { get; set; }
	public int AllElements { get; set; }
}

public sealed class DashboardUserTypesCount
{
	public DashboardUserTypesCount(int stud, int teach, int edit, int admin)
	{
		StudentsCount = stud;
		TeachersCount = teach;
		EditorsCount = edit;
		AdministratorsCount = admin;
		AllElements = stud + teach + edit + admin;
	}

	public int StudentsCount { get; set; }
	public int TeachersCount { get; set; }
	public int EditorsCount { get; set; }
	public int AdministratorsCount { get; set; }
	public int AllElements { get; set; }
}

public sealed class UserDetailsEditResDto
{
	public string Name { get; set; }
	public string Surname { get; set; }
	public string City { get; set; }
	public string Nationality { get; set; }
	public string Role { get; set; }
	public string DepartmentName { get; set; }
	public string CathedralName { get; set; }
	public List<long> StudySpecsOrSubjects { get; set; }
}
