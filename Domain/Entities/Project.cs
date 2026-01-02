namespace Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required string Name { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public User CreatedBy { get; set; }
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
        public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    }
}
