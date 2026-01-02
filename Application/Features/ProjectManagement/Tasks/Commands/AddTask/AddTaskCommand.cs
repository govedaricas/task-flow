using Application.Abstraction;

namespace Application.Features.ProjectManagement.Tasks.Commands.AddTask
{
    public class AddTaskCommand : IRequest<int>
    {
        public required string Name { get; set; }
        public int ProjectId { get; set; }
        public int? AssignedUserId { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
