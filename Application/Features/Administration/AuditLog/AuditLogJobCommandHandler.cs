using System.Text;
using Application.Interfaces;
using Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Features.Administration.AuditLog
{
    public class AuditLogJobCommandHandler
    {
        private readonly ITaskFlowDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuditLogJobCommandHandler> _logger;
        private readonly SmtpSettings _smtpSettings;

        public AuditLogJobCommandHandler(ITaskFlowDbContext dbContext, IEmailService emailService, ILogger<AuditLogJobCommandHandler> logger, IOptions<SmtpSettings> smtpSettings)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _logger = logger;
            _smtpSettings = smtpSettings.Value;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow;

            var logs = _dbContext.AuditLogs
                .Where(x => x.Timestamp >= today.AddDays(-1) && x.Timestamp < today)
                .ToList();

            if (!logs.Any())
            {
                _logger.LogInformation("No audit logs found for {Date}", today);
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("Daily Audit Log Report:");
            foreach (var log in logs)
            {
                sb.AppendLine($"{log.Id} | {log.UserId} | {log.Action} | {log.Timestamp}");
            }

            var recipient = _smtpSettings.ReportRecipientEmail;

            await _emailService.SendEmailAsync(recipient, $"Audit Log Report - {today:yyyy-MM-dd}", sb.ToString(), cancellationToken);

            _logger.LogInformation("Audit log report sent to {Recipient}", recipient);
        }
    }
}
