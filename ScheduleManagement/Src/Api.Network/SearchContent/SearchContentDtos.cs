namespace ScheduleManagement.Api.Network.SearchContent;

public sealed class SearchMassiveQueryReqDto
{
	public string SearchQuery { get; set; }
	public bool IsGroupsActive { get; set; }
	public bool IsTeachersActive { get; set; }
	public bool IsRoomsActive { get; set; }
}

public sealed class SearchMassiveQueryResDto
{
	public dynamic PathQueryParams { get; set; }
	public string DepartmentName { get; set; }
	public string PathParam { get; set; }
	public string TypeName { get; set; }
	public string FullName { get; set; }
}
