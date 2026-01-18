namespace Application.Interfaces
{
    public interface ITaskNotificationService
    {
        Task NotifyTaskStatusChanged(int taskId, string newStatus, string taskTitle, List<int> userIds);
        Task NotifyTaskCommentAdded(int taskId, string comment, string authorName, int userId);
    }
}
