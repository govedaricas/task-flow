using Application.Abstraction;
using Application.Enums;
using Application.Exceptions;
using Application.Features.ProjectManagement.ProjectStatistics.Queries.GetProjectStatistics;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;

namespace Application.Features.ProjectManagement.Tasks.Commands.ChangeTaskStatus
{
    public class ChangeTaskStatusCommandHandler : IRequestHandler<ChangeTaskStatusCommand, Domain.Entities.Task>
    {
        private readonly ITaskFlowDbContext _dbContext;
        private readonly ICacheService _cacheService;
        private readonly ITaskNotificationService _taskNotificationService;

        public ChangeTaskStatusCommandHandler(ITaskFlowDbContext dbContext, ICacheService cacheService, ITaskNotificationService taskNotificationService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
            _taskNotificationService = taskNotificationService;
        }

        public async Task<Domain.Entities.Task> Handle(ChangeTaskStatusCommand request, CancellationToken cancellationToken)
        {
            var task = await _dbContext.Tasks
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (task == null)
                throw new NotFoundException("Task", "Task not found.");

            var statusExists = await _dbContext.TaskStatuses
                .AnyAsync(x => x.Id == request.TaskStatusId, cancellationToken);

            if (!statusExists)
            {
                throw new NotFoundException("TaskStatus", "Task status not found.");
            }

            ProjectStatsDto? newStats = null;
            if (request.TaskStatusId != task.TaskStatusId)
            {
                newStats = await ChangeTaskStatusAsync(task, request.TaskStatusId, cancellationToken);
            }

            task.TaskStatusId = request.TaskStatusId;

            await _dbContext.SaveChangesAsync(cancellationToken);

            if (newStats != null)
            {
                var memberIds = await _dbContext.ProjectMembers
                    .Where(x => x.ProjectId == task.ProjectId)
                    .Select(x => x.UserId)
                    .ToListAsync(cancellationToken);

                await _taskNotificationService.NotifyProjectStatisticsChanged(task.ProjectId, newStats, memberIds);
            }

            return task;
        }

        public async Task<ProjectStatsDto?> ChangeTaskStatusAsync(Domain.Entities.Task task, byte newStatusId, CancellationToken cancellationToken)
        {
            var projectStatistics = await _dbContext.ProjectStatistics
                .FirstOrDefaultAsync(x => x.ProjectId == task.ProjectId, cancellationToken);

            if (projectStatistics == null)
                return null;

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

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };

            await _cacheService.SetAsync($"project:{task.ProjectId}:statistics", new ProjectStatisticsModel
            {
                TodoCount = projectStatistics.TodoCount,
                InProgressCount = projectStatistics.InProgressCount,
                DoneCount = projectStatistics.DoneCount,
                LastActivityAt = projectStatistics.LastActivityAt,
                IsOverloaded = projectStatistics.IsOverloaded
            }, cacheOptions, cancellationToken);

            return new ProjectStatsDto
            {
                TodoCount = projectStatistics.TodoCount,
                InProgressCount = projectStatistics.InProgressCount,
                DoneCount = projectStatistics.DoneCount,
                LastActivityAt = projectStatistics.LastActivityAt
            };
        }
    }

    public record ProjectStatsDto
    {
        public int TodoCount { get; init; }
        public int InProgressCount { get; init; }
        public int DoneCount { get; init; }
        public DateTime LastActivityAt { get; init; }
    }
}
