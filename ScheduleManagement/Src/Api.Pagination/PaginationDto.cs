namespace ScheduleManagement.Api.Pagination;

public sealed class CurrentActivePages
{
	public CurrentActivePages(int[] activePages, bool minEnabled = false, bool maxEnabled = false)
	{
		ActivePages = activePages;
		MinEnabled = minEnabled;
		MaxEnabled = maxEnabled;
	}

	public int[] ActivePages { get; set; }
	public bool MinEnabled { get; set; }
	public bool MaxEnabled { get; set; }
}

public sealed class AvailablePaginationSizes
{
	public List<int> AvailablePaginations { get; set; }
}

public sealed class DeleteSelectedRequestDto
{
	public long[] Ids { get; set; }
}