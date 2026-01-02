using Application.Abstraction;

namespace Application.Features.ProjectManagement.Tasks.Commands.UpdateTask
{
    public class UpdateTaskCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int? AssignedToUserId { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsActive { get; set; }
    }
}
