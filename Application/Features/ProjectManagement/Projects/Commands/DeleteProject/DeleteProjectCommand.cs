using Application.Abstraction;

namespace Application.Features.ProjectManagement.Projects.Commands.DeleteProject
{
    public class DeleteProjectCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
