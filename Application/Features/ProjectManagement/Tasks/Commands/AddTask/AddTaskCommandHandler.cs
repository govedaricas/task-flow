using Application.Abstraction;
using Application.Enums;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Tasks.Commands.AddTask
{
    public class AddTaskCommandHandler : IRequestHandler<AddTaskCommand, int>
    {
        private readonly ITaskFlowDbContext _dbContext;
        private readonly IUserIdentity _userIdentity;

        public AddTaskCommandHandler(ITaskFlowDbContext dbContext, IUserIdentity userIdentity)
        {
            _dbContext = dbContext;
            _userIdentity = userIdentity;
        }

        public async Task<int> Handle(AddTaskCommand request, CancellationToken cancellationToken)
        {
            var projectExists = await _dbContext.Projects
                .AnyAsync(x => x.Id == request.ProjectId && x.IsActive, cancellationToken);

            if (!projectExists)
            {
                throw new NotFoundException("Project", "Project not found.");
            }


            var task = new Domain.Entities.Task
            {
                Name = request.Name,
                AssignedUserId = null,
                CreatedByUserId = _userIdentity.Id,
                Description = request.Description,
                ProjectId = request.ProjectId,
                TaskPriorityId = (byte)TaskPriorityEnum.Low,
                DueDate = request.DueDate,
                TaskStatusId = (byte)TaskStatusEnum.New,
                CreatedAt = DateTime.UtcNow,
                IsActive = request.IsActive
            };

            var projectStatistics = await _dbContext.ProjectStatistics
                .FirstOrDefaultAsync(x => x.ProjectId == task.ProjectId, cancellationToken);

            if (projectStatistics != null)
            {
                projectStatistics.TodoCount++;
                projectStatistics.LastActivityAt = DateTime.UtcNow;
                projectStatistics.IsOverloaded = (projectStatistics.TodoCount + projectStatistics.InProgressCount) > 100;
            }

            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return task.Id;
        }
    }
}
