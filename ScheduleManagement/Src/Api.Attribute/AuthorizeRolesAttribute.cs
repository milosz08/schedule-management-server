using Microsoft.AspNetCore.Authorization;

namespace ScheduleManagement.Api.Attribute;

public class AuthorizeRolesAttribute : AuthorizeAttribute
{
	public AuthorizeRolesAttribute(params string[] allowedRoles)
	{
		Roles = string.Join(",", allowedRoles);
	}
}
