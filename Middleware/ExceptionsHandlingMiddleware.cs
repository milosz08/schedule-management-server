using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;


namespace asp_net_po_schedule_management_server.Middleware
{
    public class ExceptionsHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try {
                await next.Invoke(context);
            }
            catch (Exception err) {
                int errCode = (int)HttpStatusCode.InternalServerError;
                context.Response.StatusCode = errCode;
                await context.Response.WriteAsync(new ServerExceptionResponseDto()
                {
                    ExceptionMessage = err.Message,
                    ExceptionErrorCode = errCode,
                    ServerTimestamp = ApplicationUtils.GetTimestamp(DateTime.Now)
                }.ToString());
            }
        }
    }
}