using Application.Abstraction;
using Application.Models;
using Application.Paginations;

namespace Application.Features.ProjectManagement.Tasks.Queries.GetAllTasks
{
    public class GetAllTasksQuery : DataFilter, IRequest<PagedData<TaskModel>>
    {
        public string? Name { get; set; }
        public byte? TaskStatusId { get; set; }
        public byte? TaskPriorityId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class TaskModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int ProjectId { get; set; }
        public byte TaskStatusId { get; set; }
        public byte TaskPriorityId { get; set; }
        public int CreatedByUserId { get; set; }
        public int? AssignedUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
