using ConfigurationPlaceholders;
using DotNetEnv;

namespace ScheduleManagement.Api.Config;

public class ApiConfig
{
	public static string DbDriverVersion { get; set; }
	public static JwtConfig Jwt { get; set; }
	public static TimeSpan OtpExpiredTimestamp { get; set; }
	public static string ClientOrigin { get; set; }
	public static SshConfig? Ssh { get; set; }
	public static MailboxManagerCommandConfig? MailboxManagerCommand { get; set; }
	public static int EmailCapacityMb { get; set; }
	public static SmtpConfig Smtp { get; set; }
	public static S3Config S3 { get; set; }
	public static InitAccountConfig InitAccount { get; set; }
	public static string AboutUrl { get; set; }

	public static void BindValuesFromConfigFile(WebApplicationBuilder builder)
	{
		var environment = builder.Environment.EnvironmentName;
		IPlaceholderResolver placeholderResolver;

		if (environment == "Docker")
		{
			placeholderResolver = new EnvironmentVariableResolver();
		}
		else
		{
			var keyValuePairs = Env.Load("../.env");
			placeholderResolver = new InMemoryPlaceholderResolver(new Dictionary<string, string?>(keyValuePairs));
		}

		builder.AddConfigurationPlaceholders(placeholderResolver);
		builder.Configuration.GetSection("ApiConfig").Bind(new ApiConfig());
	}
}

public class JwtConfig
{
	public string Secret { get; set; }
	public TimeSpan ExpiredTimestamp { get; set; }
	public string Audience { get; set; }
	public string Issuer { get; set; }
}

public class SshConfig
{
	public string Server { get; set; }
	public string Username { get; set; }
	public string Password { get; set; }
}

public class MailboxManagerCommandConfig
{
	public string Create { get; set; }
	public string SetCapacity { get; set; }
	public string UpdatePassword { get; set; }
	public string Delete { get; set; }
}

public class SmtpConfig
{
	public string Host { get; set; }
	public string Port { get; set; }
	public string Username { get; set; }
	public string Password { get; set; }
	public bool EnableSsl { get; set; }
	public string EmailDomain { get; set; }
}

public class S3Config
{
	public string Url { get; set; }
	public string AccessKey { get; set; }
	public string SecretKey { get; set; }
	public string Region { get; set; }
}

public class InitAccountConfig
{
	public string Name { get; set; }
	public string Surname { get; set; }
	public string Password { get; set; }
}
