using Application.Abstraction;
using Application.Exceptions;
using Application.Features.ProjectManagement.Tasks.Queries.GetAllTasks;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Tasks.Queries.GetTaskById
{
    public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskModel>
    {
        private readonly ITaskFlowDbContext _dbContext;

        public GetTaskByIdQueryHandler(ITaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TaskModel> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var task = await _dbContext.Tasks
                .Where(t => t.Id == request.Id)
                .Select(t => new TaskModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    TaskStatusId = t.TaskStatusId,
                    TaskPriorityId = t.TaskPriorityId,
                    ProjectId = t.ProjectId,
                    AssignedUserId = t.AssignedUserId,
                    CreatedAt = t.CreatedAt,
                    CreatedByUserId = t.CreatedByUserId,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    IsActive = t.IsActive
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (task == null)
                throw new NotFoundException("Task", "Task not found.");

            return task;
        }
    }
}
