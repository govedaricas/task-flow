namespace Domain.Entities
{
    public class ProjectStatistics
    {
        public int ProjectId { get; set; }
        public int TodoCount { get; set; }
        public int InProgressCount { get; set; }
        public int DoneCount { get; set; }
        public DateTime LastActivityAt { get; set; }
        public bool IsOverloaded { get; set; }

        public Project Project { get; set; }
    }
}
