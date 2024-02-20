using System.Net;

namespace ScheduleManagement.Api.Exception;

public class ExceptionMiddlewareHandler(ILogger<ExceptionMiddlewareHandler> logger) : IMiddleware
{
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		try
		{
			await next.Invoke(context);
		}
		catch (RestApiException ex)
		{
			var statusCode = (int)ex.HttpStatusCode;
			context.Response.StatusCode = statusCode;

			logger.LogError("Rest exception: {}", ex.Message);
			await context.Response.WriteAsync(ResponseJsonValue(statusCode, ex.Message));
		}
		catch (System.Exception ex)
		{
			const int statusCode = (int)HttpStatusCode.InternalServerError;
			context.Response.StatusCode = statusCode;

			logger.LogError("System exception: {}", ex.Message);
			await context.Response.WriteAsync(ResponseJsonValue(statusCode, ex.Message));
		}
	}

	private static string ResponseJsonValue(int statusCode, string message)
	{
		return new ServerExceptionResDto
		{
			Message = message,
			ErrorCode = statusCode,
			ServerTimestamp = DateTime.Now.ToString("yyyyMMddHHmmssffff")
		}.ToString();
	}
}
