using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Administration.Auth.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, int>
    {
        private const string DefaultRoleName = "Admin";
        private const int DefaultProjectId = 1;

        private readonly IPasswordHasher _passwordHasher;
        private readonly ITaskFlowDbContext _dbContext;

        public RegisterUserCommandHandler(IPasswordHasher passwordHasher, ITaskFlowDbContext dbContext)
        {
            _passwordHasher = passwordHasher;
            _dbContext = dbContext;
        }

        public async Task<int> Handle (RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var userExists = await _dbContext.Users
                .AnyAsync(x => x.Username == request.Username, cancellationToken);

            if (userExists)
            {
                throw new ConflictException("User", "User with provided username already exists.");
            }

            var adminRole = await _dbContext.Roles
                .FirstOrDefaultAsync(x => x.Name == DefaultRoleName, cancellationToken)
                ?? throw new NotFoundException("Role", $"Role '{DefaultRoleName}' not found.");

            var defaultProjectExists = await _dbContext.Projects
                .AnyAsync(x => x.Id == DefaultProjectId, cancellationToken);

            if (!defaultProjectExists)
            {
                throw new NotFoundException("Project", $"Default project with id {DefaultProjectId} not found.");
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = Convert.FromBase64String(_passwordHasher.Hash(request.Password)),
                IsActive = request.IsActive
            };

            user.Roles.Add(adminRole);
            user.ProjectMembers.Add(new ProjectMember { ProjectId = DefaultProjectId });

            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}
