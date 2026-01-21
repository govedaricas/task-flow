using Application.Abstraction;

namespace Application.Features.ProjectManagement.ProjectStatistics.Queries.GetProjectStatistics
{
    public class GetProjectStatisticsQuery : IRequest<ProjectStatisticsModel>
    {
        public int ProjectId { get; set; }
    }
}
