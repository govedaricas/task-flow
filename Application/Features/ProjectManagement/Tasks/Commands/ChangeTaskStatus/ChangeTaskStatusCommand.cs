using Application.Abstraction;

namespace Application.Features.ProjectManagement.Tasks.Commands.ChangeTaskStatus
{
    public class ChangeTaskStatusCommand : IRequest<Domain.Entities.Task>
    {
        public int Id { get; set; }
        public byte TaskStatusId { get; set; }
    }
}
