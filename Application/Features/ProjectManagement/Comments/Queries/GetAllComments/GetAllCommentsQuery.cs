using Application.Abstraction;

namespace Application.Features.ProjectManagement.Comments.Queries.GetAllComments
{
    public class GetAllCommentsQuery : IRequest<List<CommentModel>>
    {
        public int TaskId { get; set; } 
    }
}
