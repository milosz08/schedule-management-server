using System.Linq;

using FluentValidation;

using asp_net_po_schedule_management_server.Utils;


namespace asp_net_po_schedule_management_server.Dto.Validators
{
    /// <summary>
    /// Walidator odpowiedzialny za walidację elementów paginacji
    /// </summary>
    public class UserQueryValidator : AbstractValidator<SearchQueryRequestDto>
    {
        // możliwe sortowania elementów (do wybory przez użytkownika)
        private string[] _allowedSortings =
        {
            "Id", "Surname", "Login", "Role", "Name", "Alias", "DepartmentName", "DepartmentAlias", "CathedralAlias",
            "Capacity", "RoomTypeAlias", "SpecTypeName", "SpecDegree", "SpecTypeAlias"
        };
        
        public UserQueryValidator()
        {
            RuleFor(q => q.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(q => q.PageSize).Custom((value, context) => {
                if (!ApplicationUtils._allowedPageSizes.Contains(value)) {
                    context.AddFailure(
                        "PageSize",
                        $"Ilość elementów to: [{string.Join(",", ApplicationUtils._allowedPageSizes)}]");
                }
            });
            RuleFor(p => p.SortBy)
                .Must(v => string.IsNullOrEmpty(v) || _allowedSortings.Contains(v))
                .WithMessage(
                    $"Sortowanie jest opcjonalne, ale trzeba podać jedne z: [{string.Join(",", _allowedSortings)}]");
        }
    }
}