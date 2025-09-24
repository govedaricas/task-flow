using System.Net;

namespace Application.Exceptions
{
    public abstract class BaseException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string Key { get; }

        protected BaseException(string key, string message, HttpStatusCode statusCode)
            : base(message)
        {
            Key = key;
            StatusCode = statusCode;
        }
    }
}
