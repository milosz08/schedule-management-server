using System.Linq;
using FluentValidation;

using asp_net_po_schedule_management_server.Dto.Requests;


namespace asp_net_po_schedule_management_server.Dto.Validators
{
    // walidator odpowiedzialny za walidację elementów paginacji
    public class UserQueryValidator : AbstractValidator<UserQueryRequestDto>
    {
        // możliwe ilości elementów na jednej stronie (do wybory przez użytkownika)
        private readonly int[] allowedPageSizes = new[] { 5, 10, 15 };
        
        public UserQueryValidator()
        {
            RuleFor(q => q.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(q => q.PageSize).Custom((value, context) => {
                if (!allowedPageSizes.Contains(value)) {
                    context.AddFailure(
                        "PageSize",
                        $"Ilość elementów to: [{string.Join(",", allowedPageSizes)}]");
                }
            });
        }
    }
}