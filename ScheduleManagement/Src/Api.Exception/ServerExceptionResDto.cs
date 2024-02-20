using System.Text.Json;

namespace ScheduleManagement.Api.Exception;

public class ServerExceptionResDto
{
	public string Message { get; set; }
	public int ErrorCode { get; set; }
	public string ServerTimestamp { get; set; }

	public override string ToString()
	{
		return JsonSerializer.Serialize(this);
	}
}
