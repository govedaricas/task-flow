using Application.Abstraction;

namespace Application.Features.ProjectManagement.Comments.Commands.AddComment
{
    public class AddCommentCommand : IRequest<int>
    {
        public int TaskId { get; set; }
        public required string Text { get; set; }
    }
}
