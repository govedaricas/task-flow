using System.Net;

namespace Application.Exceptions
{
    public sealed class ForbiddenException : BaseException
    {
        public ForbiddenException(string key, string message)
            : base(key, message, HttpStatusCode.Forbidden) { }
    }
}
