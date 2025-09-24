using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, int>
    {
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

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = Convert.FromBase64String(_passwordHasher.Hash(request.Password)),
                IsActive = request.IsActive
            };

            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}
