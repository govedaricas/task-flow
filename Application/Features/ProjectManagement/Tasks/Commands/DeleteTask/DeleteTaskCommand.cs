using Application.Abstraction;

namespace Application.Features.ProjectManagement.Tasks.Commands.DeleteTask
{
    public class DeleteTaskCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
