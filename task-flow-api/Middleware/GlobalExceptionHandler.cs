using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace task_flow_api.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async ValueTask<bool> TryHandleAsync(Exception exception, CancellationToken cancellationToken)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
                return false;

            ProblemDetails problemDetails = new()
            {
                Instance = httpContext.Request.Path
            };

            if (exception is BaseException appEx)
            {
                problemDetails.Status = (int)appEx.StatusCode;
                problemDetails.Title = appEx.Message;
                problemDetails.Type = appEx.Key;

                httpContext.Response.StatusCode = (int)appEx.StatusCode;
            }
            else
            {
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Server error";
                problemDetails.Type = "ServerError";
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
