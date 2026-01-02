using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Comments.Commands.AddComment
{
    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, int>
    {
        private readonly ITaskFlowDbContext _dbContext;
        private readonly IUserIdentity _userIdentity;

        public AddCommentCommandHandler(ITaskFlowDbContext dbContext, IUserIdentity userIdentity)
        {
            _dbContext = dbContext;
            _userIdentity = userIdentity;
        }

        public async Task<int> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var taskExists = await _dbContext.Tasks
                .AnyAsync(x => x.Id == request.TaskId, cancellationToken);

            if (!taskExists)
                throw new NotFoundException("Task", "Task not found for adding comment.");

            var comment = new Domain.Entities.Comment
            {
                Text = request.Text.Trim(),
                CreatedAt = DateTime.Now,
                TaskId = request.TaskId,
                AuthorId = _userIdentity.Id
            };

            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return comment.Id;
        }
    }
}
