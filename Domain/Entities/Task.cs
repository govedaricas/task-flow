namespace Domain.Entities
{
    public class Task
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int ProjectId { get; set; }
        public byte TaskStatusId { get; set; }
        public byte TaskPriorityId { get; set; }
        public int CreatedByUserId { get; set; }
        public int? AssignedUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = false;


        public Project Project { get; set; }
        public User CreatedByUser { get; set; } 
        public User? AssignedUser { get; set; } 
        public TaskStatus TaskStatus { get; set; }
        public TaskPriority TaskPriority { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }
}
