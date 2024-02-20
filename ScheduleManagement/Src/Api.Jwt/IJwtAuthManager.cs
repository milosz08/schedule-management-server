using System.Security.Claims;
using ScheduleManagement.Api.Entity;

namespace ScheduleManagement.Api.Jwt;

public interface IJwtAuthManager
{
	string BearerHandlingService(Person person);
	string BearerHandlingResetPasswordTokenService(Person person, string otpToken);
	string RefreshTokenGenerator();
	string BearerHandlingRefreshTokenService(IEnumerable<Claim> claims);
}