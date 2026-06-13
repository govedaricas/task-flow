using Application.Abstraction;
using Application.Extensions;
using Application.Interfaces;
using Application.Models;

namespace Application.Features.ProjectManagement.Comments.Queries.GetAllComments
{
    public class GetAllCommentsQueryHandler : IRequestHandler<GetAllCommentsQuery, PagedData<CommentModel>>
    {
        private readonly ITaskFlowDbContext _dbContext;

        public GetAllCommentsQueryHandler(ITaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedData<CommentModel>> Handle(GetAllCommentsQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Comments
                .Where(x => x.TaskId == request.TaskId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentModel
                {
                    Id = c.Id,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt,
                    TaskId = c.TaskId,
                    AuthorId = c.AuthorId,
                    AuthorName = c.Author.FirstName + " " + c.Author.LastName
                })
                .ApplySearchFilter(request)
                .ApplySortFilter(request)
                .ApplyPagedDataAsync(request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}
