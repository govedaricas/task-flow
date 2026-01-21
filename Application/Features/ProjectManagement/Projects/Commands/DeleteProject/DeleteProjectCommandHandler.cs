using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Projects.Commands.DeleteProject
{
    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
    {
        private readonly ITaskFlowDbContext _dbContext;

        public DeleteProjectCommandHandler(ITaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _dbContext.Projects
                .Include(x => x.Tasks)
                .Include(x => x.ProjectStatistics)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException("Project", $"Project with ID {request.Id} not found.");

            _dbContext.ProjectStatistics.RemoveRange(project.ProjectStatistics);
            _dbContext.Tasks.RemoveRange(project.Tasks);
            _dbContext.Projects.Remove(project);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
