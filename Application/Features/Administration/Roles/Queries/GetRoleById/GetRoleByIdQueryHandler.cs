using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Administration.Roles.Queries.GetRoleById
{
    internal class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleModel>
    {
        private readonly ITaskFlowDbContext _dbContext;

        public GetRoleByIdQueryHandler(ITaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<RoleModel> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Roles
                .Where(x => x.Id == request.Id)
                .Select(x => new RoleModel
                {
                    Id = x.Id,
                    Name = x.Name
                }).FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException("Role", "Role not found.");
        }
    }
}
