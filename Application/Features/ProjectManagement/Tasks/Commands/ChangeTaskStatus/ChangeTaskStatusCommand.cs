using Application.Abstraction;

namespace Application.Features.ProjectManagement.Tasks.Commands.ChangeTaskStatus
{
    public class ChangeTaskStatusCommand : IRequest<int>
    {
        public int Id { get; set; }
        public byte TaskStatusId { get; set; }
    }
}
