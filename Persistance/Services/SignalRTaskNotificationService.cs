using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Persistance.Hubs;

namespace Persistance.Services
{
    public class SignalRTaskNotificationService : ITaskNotificationService
    {
        private readonly IHubContext<TaskHub> _hubContext;

        public SignalRTaskNotificationService(IHubContext<TaskHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyTaskCommentAdded(int taskId, string comment, string authorName, int userId)
        {
            var group = $"user_{userId}";

            await _hubContext.Clients.Groups(group).SendAsync("TaskCommentAdded", new
            {
                TaskId = taskId,
                Comment = comment,
                Author = authorName,
                Timestamp = DateTime.UtcNow
            });
        }
        
        public async Task NotifyTaskStatusChanged(int taskId, string newStatus, string taskTitle, List<int> userIds)
        {
            var groups = userIds.Select(id => $"user_{id}").ToList();

            await _hubContext.Clients.Groups(groups).SendAsync("TaskStatusChanged", new
            {
                TaskId = taskId,
                Status = newStatus,
                Title = taskTitle,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
