namespace Application.Features.ProjectManagement.ProjectStatistics.Queries.GetProjectStatistics
{
    public class ProjectStatisticsModel
    {
        public int TodoCount { get; set; }
        public int InProgressCount { get; set; }
        public int OnHoldCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public DateTime LastActivityAt { get; set; }
        public bool IsOverloaded { get; set; }
    }
}
