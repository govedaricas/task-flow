using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Features.ProjectManagement.ProjectStatistics.Queries.GetProjectStatistics
{
    public class GetProjectStatisticsQueryHandler : IRequestHandler<GetProjectStatisticsQuery, ProjectStatisticsModel>
    {
        private readonly ITaskFlowDbContext _dbContext;
        private readonly IUserIdentity _userIdentity;
        private readonly ICacheService _cacheService;

        public GetProjectStatisticsQueryHandler(ITaskFlowDbContext dbContext, IUserIdentity userIdentity, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _userIdentity = userIdentity;
            _cacheService = cacheService;
        }

        public async Task<ProjectStatisticsModel> Handle(GetProjectStatisticsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"project:{request.ProjectId}:statistics";

            var cachedStatistics = await _cacheService.GetAsync<ProjectStatisticsModel>(cacheKey, cancellationToken);
            if (cachedStatistics != null)
            {
                return cachedStatistics;
            }

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

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };
            await _cacheService.SetAsync(cacheKey, projectStatistics, cacheOptions, cancellationToken);

            return projectStatistics;
        }
    }
}
