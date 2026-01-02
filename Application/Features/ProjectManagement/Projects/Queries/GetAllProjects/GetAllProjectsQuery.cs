using Application.Abstraction;
using Application.Features.ProjectManagement.Projects.Queries.GetProjectById;

namespace Application.Features.ProjectManagement.Projects.Queries.GetAllProjects
{
    public class GetAllProjectsQuery : IRequest<List<ProjectModel>>
    {
    }
}
