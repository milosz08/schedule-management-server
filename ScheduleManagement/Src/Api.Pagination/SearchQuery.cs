namespace ScheduleManagement.Api.Pagination;

public sealed class SearchQueryRequestDto
{
	public string? SearchPhrase { get; set; } = "";
	public int PageNumber { get; set; } = 1;
	public int PageSize { get; set; } = PaginationConfig.AllowedPageSizes[0];
	public string SortBy { get; set; } = "Id";
	public SortDirection SortDirection { get; set; } = SortDirection.Asc;
}

public sealed class SearchQueryResponseDto(List<string> dataElements)
{
	public SearchQueryResponseDto() : this([])
	{
	}

	public List<string> DataElements { get; set; } = dataElements;
}

public enum SortDirection
{
	Asc,
	Des
}
