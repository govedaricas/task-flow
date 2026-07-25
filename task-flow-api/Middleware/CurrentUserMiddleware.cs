using Application.Interfaces;
using Serilog.Context;

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

            var requestId = context.TraceIdentifier;
            var userId = userIdentity.Id != 0 ? userIdentity.Id.ToString() : "anonymous";

            using (LogContext.PushProperty("UserId", userId))
            using (LogContext.PushProperty("RequestId", requestId))
            {
                await _next(context);
            }
        }
    }
}
