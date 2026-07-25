using Application.Interfaces;

namespace task_flow.IntegrationTests.Infrastructure;

public class NoOpEmailService : IEmailService
{
    public Task SendEmailAsync(string toEmail, string subject, string message, CancellationToken cancellationToken)
        => Task.CompletedTask;
}

public class NoOpTaskNotificationService : ITaskNotificationService
{
    public Task NotifyTaskStatusChanged(int taskId, string newStatus, string taskTitle, List<int> userIds)
        => Task.CompletedTask;

    public Task NotifyTaskCommentAdded(int taskId, string comment, string authorName, int userId)
        => Task.CompletedTask;

    public Task NotifyProjectStatisticsChanged(int projectId, object stats, List<int> userIds)
        => Task.CompletedTask;
}
