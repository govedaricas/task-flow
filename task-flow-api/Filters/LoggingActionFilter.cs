using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace task_flow_api.Filters
{
    public class LoggingActionFilter : IAsyncActionFilter
    {
        private readonly ILogger<LoggingActionFilter> _logger;
        private const int SlowRequestThresholdMs = 500;

        public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            var path = context.HttpContext.Request.Path;

            var stopwatch = Stopwatch.StartNew();
            var executedContext = await next();
            stopwatch.Stop();

            if (executedContext.Exception != null && !executedContext.ExceptionHandled)
            {
                _logger.LogError(executedContext.Exception,
                    "{Method} {Path} failed in {ElapsedMs}ms",
                    method, path, stopwatch.ElapsedMilliseconds);
            }
            else if (stopwatch.ElapsedMilliseconds > SlowRequestThresholdMs)
            {
                _logger.LogWarning("{Method} {Path} slow request: {ElapsedMs}ms",
                    method, path, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
