using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Administration.Users.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserModel>
    {
        private readonly ITaskFlowDbContext _dbContext;

        public GetUserByIdQueryHandler(ITaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserModel> Handle (GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Select(x => new UserModel
                {
                    Id = x.Id,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Username = x.Username,
                    IsActive = x.IsActive,
                    Roles = x.Roles.ToList()
                })
                .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken)
                ?? throw new NotFoundException("User", "User not Found");

            return user;
        }
    }
}
