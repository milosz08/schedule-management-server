using FluentValidation;

using System.Text.RegularExpressions;
using asp_net_po_schedule_management_server.Dto.AuthDtos;


namespace asp_net_po_schedule_management_server.Dto.Validators
{
    public sealed class ChangePasswordRequestDtoValidator : AbstractValidator<ChangePasswordRequestDto>
    {
        public ChangePasswordRequestDtoValidator()
        {
            RuleFor(field => field.NewPassword)
                // walidacja pola nowego hasła pod względem znaków (przy pomocy wyrażenia regularnego)
                .Custom((value, context) => {
                    Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$");
                    if (!regex.IsMatch(value)) {
                        context.AddFailure("PasswordMath",
                            "Hasło musi mieć minimum 8 znaków, zawierać co najmniej jedną liczbę, " +
                            "jedną wielką literę oraz jeden znak specjalny."
                        );
                    }
                });
        }
    }
}