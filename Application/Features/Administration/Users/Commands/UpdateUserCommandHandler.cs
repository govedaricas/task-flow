using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Administration.Users.Commands
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
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            var roles = await _dbContext.Roles
                .Where(x => request.RoleIds.Contains(x.Id)).ToListAsync(cancellationToken);

            if (user == null)
            {
                throw new NotFoundException("User", "Users not found.");
            }

            if (roles.Count != request.RoleIds.Count)
            {
                throw new NotFoundException("Role", "Roles not found.");
            }

            user.Username = request.Username;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.IsActive = request.IsActive;

            user.Roles.Clear();
            user.Roles = roles;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
