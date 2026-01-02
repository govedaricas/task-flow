using Application.Abstraction;

namespace Application.Features.ProjectManagement.Comments.Commands.UpdateComment
{
    public class UpdateCommentCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public required string Text { get; set; }
    }
}
