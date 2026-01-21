using Application.Abstraction;
using Application.Enums;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Tasks.Commands.DeleteTask
{
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
    {
        private readonly ITaskFlowDbContext _dbContext;

        public DeleteTaskCommandHandler(ITaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (task == null)
                throw new NotFoundException("Task", "Task not found.");

            var projectStatistics = await _dbContext.ProjectStatistics
                .FirstOrDefaultAsync(x => x.ProjectId == task.ProjectId, cancellationToken);

            if (projectStatistics != null)
            {
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

                projectStatistics.LastActivityAt = DateTime.UtcNow;
                projectStatistics.IsOverloaded = (projectStatistics.TodoCount + projectStatistics.InProgressCount) > 100;
            }

            _dbContext.Tasks.Remove(task);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
