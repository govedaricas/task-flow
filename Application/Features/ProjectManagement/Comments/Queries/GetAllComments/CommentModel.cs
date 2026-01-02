namespace Application.Features.ProjectManagement.Comments.Queries.GetAllComments
{
    public class CommentModel
    {
        public int Id { get; set; }
        public required string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TaskId { get; set; }
        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }
    }
}
