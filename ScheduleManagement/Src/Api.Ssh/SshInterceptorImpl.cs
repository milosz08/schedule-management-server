using System.Net;
using System.Net.Sockets;
using Renci.SshNet;
using Renci.SshNet.Common;
using ScheduleManagement.Api.Config;
using ScheduleManagement.Api.Exception;
using ConnectionInfo = Renci.SshNet.ConnectionInfo;

namespace ScheduleManagement.Api.Ssh;

public class SshInterceptorImpl : ISshInterceptor
{
	private readonly SshClient? _sshClient;

	public SshInterceptorImpl(ILogger<SshInterceptorImpl> logger)
	{
		if (ApiConfig.Ssh?.Enabled == false)
		{
			logger.LogInformation("Skipping SSH configuration (SSH is disabled)");
			return;
		}

		var keyAuth = new KeyboardInteractiveAuthenticationMethod(ApiConfig.Ssh?.Username);
		keyAuth.AuthenticationPrompt += HandleKeyEvent;
		var connectionInfo = new ConnectionInfo(ApiConfig.Ssh?.Server, 22, ApiConfig.Ssh?.Username, keyAuth);
		_sshClient = new SshClient(connectionInfo);
	}

	public void ExecuteCommand(string commandParameter)
	{
		if (_sshClient == null) return;

		try
		{
			_sshClient.Connect();
			var customCommand = _sshClient.CreateCommand(commandParameter);
			customCommand.Execute();
		}
		catch (SocketException ex)
		{
			throw new RestApiException(
				$"Nieudane połączenie z socketem SSH. Komunikat błędu: {ex}",
				HttpStatusCode.GatewayTimeout);
		}
		catch (SshOperationTimeoutException ex)
		{
			throw new RestApiException(
				$"Zbyt długi czas wykonywania komendy. Potencjalny problem z połączeniem. Komunikat błędu: {ex}",
				HttpStatusCode.GatewayTimeout);
		}
		finally
		{
			_sshClient.Disconnect();
		}
	}

	private static void HandleKeyEvent(object? sender, AuthenticationPromptEventArgs eventArgs)
	{
		foreach (var prompt in eventArgs.Prompts)
			if (prompt.Request.Contains("Password", StringComparison.InvariantCultureIgnoreCase))
				prompt.Response = ApiConfig.Ssh?.Password;
	}
}
