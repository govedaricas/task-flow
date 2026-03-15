using Application.Features.ProjectManagement.ProjectStatistics.Queries.GetProjectStatistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace task_flow_api.Controllers
{
    [Route("api/project-management/project-statistics")]
    [ApiController]
    public class ProjectStatisticsController : ControllerBase
    {
        private readonly GetProjectStatisticsQueryHandler _getProjectStatisticsQueryHandler;

        public ProjectStatisticsController(GetProjectStatisticsQueryHandler getProjectStatisticsQueryHandler)
        {
            _getProjectStatisticsQueryHandler = getProjectStatisticsQueryHandler;
        }

        [Authorize()]
        [HttpGet("{projectId}/statistics")]
        public async Task<ProjectStatisticsModel> GetStatistics(int projectId, CancellationToken cancellationToken)
        {
            return await _getProjectStatisticsQueryHandler.Handle(new GetProjectStatisticsQuery { ProjectId = projectId }, cancellationToken);
        }
    }
}
