using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Tasks.Commands.ChangeTaskStatus
{
    public class ChangeTaskStatusCommandHandler : IRequestHandler<ChangeTaskStatusCommand, Domain.Entities.Task>
    {
        private readonly ITaskFlowDbContext _dbContext;

        public ChangeTaskStatusCommandHandler(ITaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Domain.Entities.Task> Handle(ChangeTaskStatusCommand request, CancellationToken cancellationToken)
        {
            var task = await _dbContext.Tasks
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (task == null)
                throw new NotFoundException("Task", "Task not found.");

            var statusExists = await _dbContext.TaskStatuses
                .AnyAsync(x => x.Id ==  request.TaskStatusId, cancellationToken);
            
            if (!statusExists)
            {
                throw new NotFoundException("TaskStatus", "Task status not found.");
            }

            task.TaskStatusId = request.TaskStatusId;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return task;
        }
    }
}
