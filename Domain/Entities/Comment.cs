namespace Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public required string Text { get; set; }
        public required DateTime CreatedAt { get; set; } 
        public int TaskId { get; set; }
        public int AuthorId { get; set; }

        public Task Task { get; set; } 
        public User Author { get; set; }
    }
}
