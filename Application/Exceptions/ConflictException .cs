using System.Net;

namespace Application.Exceptions
{
    public sealed class ConflictException : BaseException
    {
        public ConflictException(string key, string message)
            : base(key, message, HttpStatusCode.Conflict) { }
    }
}
