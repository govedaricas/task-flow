using Application.Abstraction;
using Application.Features.ProjectManagement.Projects.Queries.GetProjectById;
using Application.Models;
using Application.Paginations;

namespace Application.Features.ProjectManagement.Projects.Queries.GetAllProjects
{
    public class GetAllProjectsQuery : DataFilter, IRequest<PagedData<ProjectModel>>
    {
    }
}
