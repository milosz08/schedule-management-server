using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ScheduleManagement.Api.Config;
using ScheduleManagement.Api.Entity;

namespace ScheduleManagement.Api.Jwt;

public class JwtAuthManagerImpl : IJwtAuthManager
{
	private static readonly byte[] TokenKey = Encoding.ASCII.GetBytes(ApiConfig.Jwt.Secret);

	public string BearerHandlingService(Person person)
	{
		return JwtDeploymentDescriptor(new[]
		{
			new Claim(ClaimTypes.Name, person.Login),
			new Claim(ClaimTypes.Role, person.Role.Name)
		}, ApiConfig.Jwt.ExpiredTimestamp);
	}

	public string BearerHandlingResetPasswordTokenService(Person person, string otpToken)
	{
		return JwtDeploymentDescriptor(new[]
		{
			new Claim(ClaimTypes.Name, person.Login),
			new Claim(ClaimTypes.Role, person.Role.Name),
			new Claim(ClaimTypes.Rsa, otpToken)
		}, ApiConfig.OtpExpiredTimestamp);
	}

	public string RefreshTokenGenerator()
	{
		var randomNumbers = new byte[32];
		RandomNumberGenerator.Create().GetBytes(randomNumbers);
		return Convert.ToBase64String(randomNumbers);
	}

	public string BearerHandlingRefreshTokenService(IEnumerable<Claim> claims)
	{
		return JwtDeploymentDescriptor(claims, ApiConfig.Jwt.ExpiredTimestamp);
	}

	public static TokenValidationParameters GetBasicTokenValidationParameters(bool isValidateLifetime = true)
	{
		return new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(TokenKey),
			ValidateAudience = false,
			ValidateIssuer = false,
			ValidateLifetime = isValidateLifetime
		};
	}

	private static string JwtDeploymentDescriptor(IEnumerable<Claim> claims, TimeSpan tokenExpired)
	{
		var handler = new JwtSecurityTokenHandler();
		var descriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.Add(tokenExpired),
			Audience = ApiConfig.Jwt.Audience,
			Issuer = ApiConfig.Jwt.Issuer,
			IssuedAt = DateTime.UtcNow,
			SigningCredentials = new SigningCredentials(
				new SymmetricSecurityKey(TokenKey),
				SecurityAlgorithms.HmacSha256Signature
			)
		};
		return handler.WriteToken(handler.CreateToken(descriptor));
	}
}
