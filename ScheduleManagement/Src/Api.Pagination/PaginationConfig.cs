using System.Linq.Expressions;

namespace ScheduleManagement.Api.Pagination;

public static class PaginationConfig
{
	public static readonly int[] AllowedPageSizes = [5, 10, 15, 30];

	public static List<T> ConfigureAdditionalFiltering<T>(IQueryable<T> baseQuery, SearchQueryRequestDto query)
	{
		return baseQuery
			.Skip(query.PageSize * (query.PageNumber - 1))
			.Take(query.PageSize)
			.ToList();
	}

	public static void ConfigureSorting<T>(Dictionary<string, Expression<Func<T, object>>> colSelect,
		SearchQueryRequestDto query, ref IQueryable<T> baseQuery)
	{
		var selectColumn = colSelect.FirstOrDefault(c => c.Key.Equals(query.SortBy)).Value;
		if (selectColumn != null)
			baseQuery = query.SortDirection == SortDirection.Asc
				? baseQuery.OrderBy(selectColumn)
				: baseQuery.OrderByDescending(selectColumn);
	}
}
