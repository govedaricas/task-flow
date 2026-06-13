using Application.Abstraction;
using Application.Models;
using Application.Paginations;

namespace Application.Features.ProjectManagement.Comments.Queries.GetAllComments
{
    public class GetAllCommentsQuery : DataFilter, IRequest<PagedData<CommentModel>>
    {
        public int TaskId { get; set; } 
    }
}
