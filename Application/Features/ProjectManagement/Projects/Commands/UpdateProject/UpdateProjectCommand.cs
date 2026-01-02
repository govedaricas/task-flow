using Application.Abstraction;

namespace Application.Features.ProjectManagement.Projects.Commands.UpdateProject
{
    public class UpdateProjectCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
