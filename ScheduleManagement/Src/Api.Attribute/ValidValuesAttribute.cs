using System.ComponentModel.DataAnnotations;

namespace ScheduleManagement.Api.Attribute;

public class ValidValuesAttribute(params string[] args) : ValidationAttribute
{
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		return args.Contains(value as string)
			? ValidationResult.Success
			: new ValidationResult("Podana wartość nie jest zadeklarowana jako wartość akceptowalna");
	}
}
