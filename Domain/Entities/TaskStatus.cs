namespace Domain.Entities
{
    public class TaskStatus
    {
        public byte Id { get; set; }
        public required string Name { get; set; }

        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
