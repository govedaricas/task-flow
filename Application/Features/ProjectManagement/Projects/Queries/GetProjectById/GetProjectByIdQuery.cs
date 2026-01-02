using Application.Abstraction;

namespace Application.Features.ProjectManagement.Projects.Queries.GetProjectById
{
    public class GetProjectByIdQuery : IRequest<ProjectModel>
    {
        public int Id { get; set; }
    }

    public class ProjectModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public int CreatedById { get; set; }
    }
}
