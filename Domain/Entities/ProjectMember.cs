namespace Domain.Entities
{
    public class ProjectMember
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }

        public Project Project { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
