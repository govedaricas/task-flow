using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly ITaskFlowDbContext _dbContext;

        public UpdateUserCommandHandler(ITaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (user == null)
            {
                throw new NotFoundException("User", "Users not found.");
            }

            user.Username = request.Username;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.IsActive = request.IsActive;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
