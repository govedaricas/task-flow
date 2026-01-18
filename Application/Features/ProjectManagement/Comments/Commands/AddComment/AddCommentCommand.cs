using Application.Abstraction;
using Domain.Entities;

namespace Application.Features.ProjectManagement.Comments.Commands.AddComment
{
    public class AddCommentCommand : IRequest<Comment>
    {
        public int TaskId { get; set; }
        public required string Text { get; set; }
    }
}
