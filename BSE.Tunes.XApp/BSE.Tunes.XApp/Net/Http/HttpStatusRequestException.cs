using System.Net;

namespace BSE.Tunes.XApp.Net.Http
{
    public class HttpStatusRequestException : System.Net.Http.HttpRequestException
    {
        public HttpStatusCode StatusCode { get; }
        
        public HttpStatusRequestException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
