using ScheduleManagement.Api.Dto;

namespace ScheduleManagement.Api.Network.ScheduleSubject;

public sealed class ScheduleDataRes
{
	public List<string> TraceDetails { get; set; } = [];
	public string ScheduleHeaderData { get; set; }
	public string CurrentChooseWeek { get; set; }
	public List<ScheduleCanvasData> ScheduleCanvasData { get; set; } = [];
}

public sealed class ScheduleCanvasData
{
	public NameIdElementDto WeekdayNameWithId { get; set; }
	public string WeekdayDateTime { get; set; }
	public bool IsNotShowingOccuredDates { get; set; }
	public List<WeekdayData> WeekdayData { get; set; } = [];
}

public class WeekdayData
{
	public long ScheduleSubjectId { get; set; }
	public string SubjectWithTypeAlias { get; set; }
	public string SubjectTypeHexColor { get; set; }
	public string SubjectTime { get; set; }
	public int PositionFromTop { get; set; }
	public int ElementHeight { get; set; }
	public string SubjectOccuredData { get; set; }
	public bool IsNotShowingOccuredDates { get; set; }
	public Dictionary<string, List<AliasData>> Aliases { get; set; }
}

public sealed class AliasData
{
	public string Alias { get; set; }
	public Dictionary<string, long> PathValues { get; set; }
}

public class ScheduleGroupQuery
{
	public long DeptId { get; set; }
	public long SpecId { get; set; }
	public long GroupId { get; set; }
}

public class ScheduleEmployerQuery
{
	public long DeptId { get; set; }
	public long CathId { get; set; }
	public long EmployerId { get; set; }
}

public class ScheduleRoomQuery
{
	public long DeptId { get; set; }
	public long CathId { get; set; }
	public long RoomId { get; set; }
}

public sealed class ScheduleActivityReqDto
{
	public long DeptId { get; set; }
	public long StudySpecId { get; set; }
	public long StudyGroupId { get; set; }
	public bool IsAddForAllGroups { get; set; }
	public string StudyYear { get; set; }
	public long WeekDayId { get; set; }
	public string SubjectOrActivityName { get; set; }
	public string SubjectTypeName { get; set; }
	public List<long> SubjectRooms { get; set; }
	public List<long> SubjectTeachers { get; set; }
	public string HourStart { get; set; }
	public string HourEnd { get; set; }
	public List<string> WeeksData { get; set; }
}

public sealed class ScheduleSubjectDetailsResDto
{
	public long Id { get; set; }
	public string SubjectName { get; set; }
	public string SubjectTypeColor { get; set; }
	public string SubjectHours { get; set; }
	public string Teachers { get; set; }
	public string TypeAndRoomsName { get; set; }
	public string DepartmentName { get; set; }
	public string SubjectOccur { get; set; }
}

public sealed class ScheduleFilteringData
{
	public string SelectedYears { get; set; }
	public string WeekInputOptions { get; set; }
}
