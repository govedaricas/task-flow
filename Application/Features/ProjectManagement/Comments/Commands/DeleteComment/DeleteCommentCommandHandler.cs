using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Comments.Commands.DeleteComment
{
    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, bool>
    {
        private readonly ITaskFlowDbContext _dbContext;
        private readonly IUserIdentity _userIdentity;

        public DeleteCommentCommandHandler(ITaskFlowDbContext dbContext, IUserIdentity userIdentity)
        {
            _dbContext = dbContext;
            _userIdentity = userIdentity;
        }

        public async Task<bool> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _dbContext.Comments
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (comment == null)
                throw new NotFoundException("Comment", "Comment not found.");

            // samo autor ili admin može obrisati
            if (comment.AuthorId != _userIdentity.Id)
                throw new ForbiddenException("Comment", "You cannot delete this comment.");

            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
