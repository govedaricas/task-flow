using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Projects.Commands.UpdateProject
{
    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, bool>
    {
        private readonly ITaskFlowDbContext _dbContext;

        public UpdateProjectCommandHandler(ITaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _dbContext.Projects
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException("Project", $"Project with ID {request.Id} not found.");

            project.Name = request.Name;
            project.Description = request.Description;
            project.IsActive = request.IsActive;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
