using System.Security.Claims;
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

        public int? Id
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(value, out var id))
                    return id;

                return null; 
            }
        }

        public string? Username
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            }
        }
    }
}
