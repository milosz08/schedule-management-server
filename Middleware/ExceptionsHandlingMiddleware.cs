using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Exceptions;


namespace asp_net_po_schedule_management_server.Middleware
{
    public sealed class ExceptionsHandlingMiddleware : IMiddleware
    {
        
        // Middleware przechwytujący wszystkie złapane wyjątki do odpowiednich klauzur catch i wysyłających
        // do użytkownika obiekt JSON z informacją o błędzie
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try {
                await next.Invoke(context);
            }
            catch (BasicServerException err) {
                int statusCode = (int)err.HttpStatusCode;
                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsync(ResponseJsonValue(statusCode, err.Message));
            }
            catch (Exception err) {
                int statusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsync(ResponseJsonValue(statusCode, err.Message));
            }
        }

        
        private string ResponseJsonValue(int statusCode, string message)
        {
            return new ServerExceptionResponseDto()
            {
                ExceptionMessage = message,
                ExceptionErrorCode = statusCode,
                ServerTimestamp = ApplicationUtils.GetTimestamp(DateTime.Now)
            }.ToString();
        }
    }
}