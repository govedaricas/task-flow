using Application.Abstraction;

namespace Application.Features.ProjectManagement.Comments.Commands.DeleteComment
{
    public class DeleteCommentCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
