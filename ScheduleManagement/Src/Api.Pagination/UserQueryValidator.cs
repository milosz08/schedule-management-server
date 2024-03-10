using FluentValidation;

namespace ScheduleManagement.Api.Pagination;

public class UserQueryValidator : AbstractValidator<SearchQueryRequestDto>
{
	private readonly string[] _allowedSortings =
	[
		"Id", "Surname", "Login", "Role", "Name", "Alias", "DepartmentName", "DepartmentAlias", "CathedralAlias",
		"Capacity", "RoomTypeAlias", "SpecTypeName", "SpecDegree", "SpecTypeAlias", "IssueType", "IsAnonymous",
		"CreatedDate"
	];

	public UserQueryValidator()
	{
		RuleFor(q => q.PageNumber).GreaterThanOrEqualTo(1);
		RuleFor(q => q.PageSize).Custom((value, context) =>
		{
			if (!PaginationConfig.AllowedPageSizes.Contains(value))
			{
				context.AddFailure(
					"PageSize",
					$"Ilość elementów to: [{string.Join(",", PaginationConfig.AllowedPageSizes)}]");
			}
		});
		RuleFor(p => p.SortBy)
			.Must(v => string.IsNullOrEmpty(v) || _allowedSortings.Contains(v))
			.WithMessage(
				$"Sortowanie jest opcjonalne, ale trzeba podać jedne z: [{string.Join(",", _allowedSortings)}]");
	}
}
