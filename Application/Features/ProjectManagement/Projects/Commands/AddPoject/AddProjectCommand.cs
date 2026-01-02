using Application.Abstraction;

namespace Application.Features.ProjectManagement.Projects.Commands.AddPoject
{
    public class AddProjectCommand : IRequest<int>
    {
        public required string Code { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
