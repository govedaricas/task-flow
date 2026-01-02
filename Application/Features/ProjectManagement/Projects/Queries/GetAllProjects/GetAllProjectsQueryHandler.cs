using Application.Abstraction;
using Application.Features.ProjectManagement.Projects.Queries.GetProjectById;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Projects.Queries.GetAllProjects
{
    public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, List<ProjectModel>>
    {
        private readonly ITaskFlowDbContext _dbContext;

        public GetAllProjectsQueryHandler(ITaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProjectModel>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Projects
                .Select(p => new ProjectModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    CreatedById = p.CreatedById,
                    IsActive = p.IsActive
                })
                .ToListAsync(cancellationToken);
        }
    }
}
