using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Comments.Commands.UpdateComment
{
    public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, bool>
    {
        private readonly ITaskFlowDbContext _dbContext;
        private readonly IUserIdentity _userIdentity;

        public UpdateCommentCommandHandler(ITaskFlowDbContext dbContext, IUserIdentity userIdentity)
        {
            _dbContext = dbContext;
            _userIdentity = userIdentity;
        }

        public async Task<bool> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _dbContext.Comments
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (comment == null)
                throw new NotFoundException("Comment", "Comment not found.");

            if (comment.AuthorId != _userIdentity.Id)
                throw new ForbiddenException("Comment", "You can only edit your own comments.");

            comment.Text = request.Text.Trim();
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
