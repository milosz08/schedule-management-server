using System.Text.Json.Serialization;

namespace ScheduleManagement.Api.Pagination;

public sealed class PaginationResponseDto<T>
{
	[JsonIgnore] private const int MaxPagesPlaceholder = 3;

	public PaginationResponseDto(List<T> elements, int totalCount, int pageSize, int pageNumber)
	{
		Elements = elements;
		TotalElementsCount = totalCount;
		ElementsFrom = pageSize * (pageNumber - 1) + 1;
		ElementsTo = ElementsFrom + pageSize - 1;
		var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
		TotalPagesCount = totalPages;
		CurrentActivePages = ComputedCurrentPageRange(pageNumber, totalPages);
	}

	public List<T> Elements { get; set; }
	public int TotalPagesCount { get; set; }
	public int ElementsFrom { get; set; }
	public int ElementsTo { get; set; }
	public int TotalElementsCount { get; set; }
	public CurrentActivePages CurrentActivePages { get; set; }

	private CurrentActivePages ComputedCurrentPageRange(int currentPage, int maxPagesCount)
	{
		var pages = new int[MaxPagesPlaceholder];
		if (maxPagesCount < MaxPagesPlaceholder)
		{
			return new CurrentActivePages(new int[maxPagesCount].Select((_, i) => i + 1).ToArray());
		}
		if (currentPage >= 1 && currentPage < pages.Length)
		{
			// first 4
			return new CurrentActivePages(pages.Select((_, i) => i + 1).Append(4).ToArray(), false, true);
		}
		if (currentPage > maxPagesCount - (pages.Length - 1) && currentPage <= maxPagesCount)
		{
			// last 4
			return new CurrentActivePages(pages
				.Select((_, i) => maxPagesCount + (i - (pages.Length - 1))).ToArray(), true);
		}
		// all others
		return new CurrentActivePages([currentPage - 1, currentPage, currentPage + 1], true, true);
	}
}