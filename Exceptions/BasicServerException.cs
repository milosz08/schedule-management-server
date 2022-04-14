using System;
using System.Net;


namespace asp_net_po_schedule_management_server.Exceptions
{
    public class BasicServerException : Exception
    {
        private HttpStatusCode _httpStatusCode;
        
        public BasicServerException(string message, HttpStatusCode httpStatusCode) : base(message)
        {
            _httpStatusCode = httpStatusCode;
        }

        public HttpStatusCode HttpStatusCode => _httpStatusCode;
    }
}