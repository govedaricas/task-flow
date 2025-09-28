using Application.Features.Administration.AuditLog;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace Application.BackgroundJobs
{
    public class BackgroundJobHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IRecurringJobManager _recurringJobManager;

        public BackgroundJobHandler(IServiceProvider serviceProvider, IRecurringJobManager recurringJobManager)
        {
            _serviceProvider = serviceProvider;
            _recurringJobManager = recurringJobManager;
        }

        public void RegisterJobs()
        {
            using var scope = _serviceProvider.CreateScope();

            var auditLogHandler = scope.ServiceProvider.GetRequiredService<AuditLogJobCommandHandler>();

            _recurringJobManager.AddOrUpdate(
                "daily-auditlog-report",
                () => auditLogHandler.ExecuteAsync(CancellationToken.None),
                "0 7 * * *" 
            );
        }
    }
}
