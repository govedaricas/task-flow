using Application.Abstraction;

namespace Application.Features.ProjectManagement.Tasks.Queries.GetTaskAuditLogs
{
    public class GetTaskAuditLogsQuery : IRequest<List<TaskAuditModel>>
    {
        public int TaskId { get; set; }
    }

    public class TaskAuditModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string EntityId { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? Changes { get; set; }
    }
}
