using System.Security.Claims;
using Application.Exceptions;
using Application.Interfaces;

namespace task_flow_api.Identity
{
    public class UserIdentity : IUserIdentity
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserIdentity(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? Id => int.TryParse(_httpContextAccessor.HttpContext?
            .User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? 
                id : throw new ConflictException("Context", "User context is unavaliable.");

        public string? Username => _httpContextAccessor.HttpContext?
            .User?.Identity?.Name ?? throw new ConflictException("Context", "User context is unavaliable.");
    }
}
