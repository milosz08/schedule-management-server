using System.Net;

namespace ScheduleManagement.Api.Exception;

public class RestApiException : System.Exception
{
	public RestApiException(string message, HttpStatusCode httpStatusCode) : base(message)
	{
		HttpStatusCode = httpStatusCode;
	}

	public HttpStatusCode HttpStatusCode { get; }
}
