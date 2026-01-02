using Application.Abstraction;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Tasks.Queries.GetAllTasks
{
    public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, List<TaskModel>>
    {
        private readonly ITaskFlowDbContext _dbContext;
        private readonly IUserIdentity _userIdentity;

        public GetAllTasksQueryHandler(ITaskFlowDbContext dbContext, IUserIdentity userIdentity)
        {
            _dbContext = dbContext;
            _userIdentity = userIdentity;
        }

        public async Task<List<TaskModel>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Tasks
                .Where(x => x.Project.Members.Any(y => y.UserId == _userIdentity.Id))
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
