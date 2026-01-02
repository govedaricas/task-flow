using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Projects.Commands.AddPoject
{
    public class AddProjectCommandHandler : IRequestHandler<AddProjectCommand, int>
    {
        private readonly ITaskFlowDbContext _dbContext;
        private readonly IUserIdentity _userIdentity;

        public AddProjectCommandHandler(ITaskFlowDbContext dbContext, IUserIdentity userIdentity)
        {
            _dbContext = dbContext;
            _userIdentity = userIdentity;
        }

        public async Task<int> Handle(AddProjectCommand request, CancellationToken cancellationToken)
        {
            var projectNameInUse = await _dbContext.Projects
                .AnyAsync(x => x.Name == request.Name || x.Code == request.Code, cancellationToken);

            if (projectNameInUse)
            {
                throw new ConflictException("Project", "Project with provided code or name already exists.");
            }

            var project = new Project
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                CreatedById = _userIdentity.Id,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _dbContext.Projects.Add(project);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return project.Id;
        }
    }
}
