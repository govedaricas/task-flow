using Application.Abstraction;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Tasks.Queries.GetAllTasks
{
    public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, List<TaskModel>>
    {
        private readonly ITaskFlowDbContext _dbContext;

        public GetAllTasksQueryHandler(ITaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TaskModel>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Tasks
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
                .ToListAsync(cancellationToken);
        }
    }
}
