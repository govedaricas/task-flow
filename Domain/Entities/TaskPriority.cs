namespace Domain.Entities
{
    public class TaskPriority
    {
        public byte Id { get; set; }
        public required string Name { get; set; }
        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
