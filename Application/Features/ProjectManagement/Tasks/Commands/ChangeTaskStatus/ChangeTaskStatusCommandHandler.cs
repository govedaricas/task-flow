using Application.Abstraction;
using Application.Enums;
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

            if (request.TaskStatusId != task.TaskStatusId)
            {
                ChangeTaskStatusAsync(task, request.TaskStatusId, cancellationToken);
            }

            task.TaskStatusId = request.TaskStatusId;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return task;
        }

        public async void ChangeTaskStatusAsync (Domain.Entities.Task task, byte newStatusId, CancellationToken cancellationToken)
        {
            var projectStatistics = await _dbContext.ProjectStatistics
                .FirstOrDefaultAsync(x => x.ProjectId == task.ProjectId, cancellationToken);

            if (projectStatistics == null)
                return;

            // Decrementing old status count
            switch ((TaskStatusEnum)task.TaskStatusId)
            {
                case TaskStatusEnum.New:
                    projectStatistics.TodoCount--;
                    break;

                case TaskStatusEnum.InProgress:
                case TaskStatusEnum.OnHold:
                    projectStatistics.InProgressCount--;
                    break;

                case TaskStatusEnum.Completed:
                    projectStatistics.DoneCount--;
                    break;
            }

            // Incrementing new status count
            switch ((TaskStatusEnum)newStatusId)
            {
                case TaskStatusEnum.New:
                    projectStatistics.TodoCount++;
                    break;

                case TaskStatusEnum.InProgress:
                case TaskStatusEnum.OnHold:
                    projectStatistics.InProgressCount++;
                    break;

                case TaskStatusEnum.Completed:
                    projectStatistics.DoneCount++;
                    break;
            }

            projectStatistics.LastActivityAt = DateTime.UtcNow;
            projectStatistics.IsOverloaded = (projectStatistics.TodoCount + projectStatistics.InProgressCount) > 100;
        }
    }
}
