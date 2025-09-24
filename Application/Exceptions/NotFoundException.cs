using System.Net;

namespace Application.Exceptions
{
    public sealed class NotFoundException : BaseException
    {
        public NotFoundException(string key, string message)
            : base(key, message, HttpStatusCode.NotFound) { }
    }
}
