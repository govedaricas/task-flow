using Application.Abstraction;
using Application.Extensions;
using Application.Features.ProjectManagement.Projects.Queries.GetProjectById;
using Application.Interfaces;
using Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Projects.Queries.GetAllProjects
{
    public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, PagedData<ProjectModel>>
    {
        private readonly ITaskFlowDbContext _dbContext;
        private readonly IUserIdentity _userIdentity;

        public GetAllProjectsQueryHandler(ITaskFlowDbContext dbContext, IUserIdentity userIdentity)
        {
            _dbContext = dbContext;
            _userIdentity = userIdentity;
        }

        public async Task<PagedData<ProjectModel>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
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
                .ApplySearchFilter(request)
                .ApplySortFilter(request)
                .ApplyPagedDataAsync(request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}
