using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.ProjectStatistics.Queries.GetProjectStatistics
{
    public class GetProjectStatisticsQueryHandler : IRequestHandler<GetProjectStatisticsQuery, ProjectStatisticsModel>
    {
        private readonly ITaskFlowDbContext _dbContext;
        private readonly IUserIdentity _userIdentity;

        public GetProjectStatisticsQueryHandler(ITaskFlowDbContext dbContext, IUserIdentity userIdentity)
        {
            _dbContext = dbContext;
            _userIdentity = userIdentity;
        }

        public async Task<ProjectStatisticsModel> Handle(GetProjectStatisticsQuery request, CancellationToken cancellationToken)
        {
            var projectStatistics = await _dbContext.ProjectStatistics
                .Where(x => x.ProjectId ==  request.ProjectId && x.Project.Members.Any(y => y.UserId == _userIdentity.Id))
                .Select(x => new ProjectStatisticsModel
                {
                    DoneCount = x.DoneCount,
                    InProgressCount = x.InProgressCount,
                    TodoCount = x.TodoCount,
                    LastActivityAt = x.LastActivityAt,
                    IsOverloaded = x.IsOverloaded
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (projectStatistics == null)
            {
                throw new NotFoundException("ProjectStatistics", "Project statistics not found.");
            }

            return projectStatistics;
        }
    }
}
