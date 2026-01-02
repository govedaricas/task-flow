using Application.Interfaces;

namespace task_flow_api.Middleware
{
    public class CurrentUserMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITaskFlowDbContext dbContext, IUserIdentity userIdentity)
        {
            if (userIdentity.Id != 0)
            {
                dbContext.CurrentUser = userIdentity; 
            }

            await _next(context);
        }
    }
}
