using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using asp_net_po_schedule_management_server.Dto;
using asp_net_po_schedule_management_server.Utils;
using asp_net_po_schedule_management_server.Exceptions;


namespace asp_net_po_schedule_management_server.Middleware
{
    /// <summary>
    /// Klasa przechowytująca wyjątki rzucane w całej aplikacji.
    /// </summary>
    public sealed class ExceptionsHandlingMiddleware : IMiddleware
    {
        /// <summary>
        /// Middleware przechwytujący wszystkie złapane wyjątki do odpowiednich klauzur catch i wysyłających
        /// do użytkownika obiekt JSON z informacją o błędzie.
        /// </summary>
        /// <param name="context">kontekst zapytania</param>
        /// <param name="next">wywołanie następnej instrukcji w potoku głównym aplikacji</param>
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

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Metoda pomocnicza tworząca i zwracająca obiekt informacji o stanie serwera.
        /// </summary>
        /// <param name="statusCode">kod statusu serwera</param>
        /// <param name="message">wiadomosć</param>
        /// <returns>wyjątek sparsowany na stringa</returns>
        private string ResponseJsonValue(int statusCode, string message)
        {
            return new ServerExceptionResponseDto()
            {
                Message = message,
                ErrorCode = statusCode,
                ServerTimestamp = ApplicationUtils.GetTimestamp(DateTime.Now)
            }.ToString();
        }
    }
}