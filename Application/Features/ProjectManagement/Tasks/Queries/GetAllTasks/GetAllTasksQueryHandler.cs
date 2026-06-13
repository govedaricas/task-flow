using Application.Abstraction;
using Application.Extensions;
using Application.Interfaces;
using Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Tasks.Queries.GetAllTasks
{
    public class GetAllTasksQueryHandler : IRequestHandler<GetAllTasksQuery, PagedData<TaskModel>>
    {
        private readonly ITaskFlowDbContext _dbContext;
        private readonly IUserIdentity _userIdentity;

        public GetAllTasksQueryHandler(ITaskFlowDbContext dbContext, IUserIdentity userIdentity)
        {
            _dbContext = dbContext;
            _userIdentity = userIdentity;
        }

        public async Task<PagedData<TaskModel>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Tasks
                .Where(x => x.Project.Members.Any(y => y.UserId == _userIdentity.Id) &&
                            (!request.TaskPriorityId.HasValue || x.TaskPriorityId == request.TaskPriorityId) &&
                            (!request.TaskStatusId.HasValue || x.TaskStatusId == request.TaskStatusId) &&
                            (string.IsNullOrEmpty(request.Name) || x.Name.Equals(request.Name)) &&
                            (!request.CreatedAt.HasValue || x.CreatedAt == request.CreatedAt))
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
                .ApplySortFilter(request)
                .ApplySearchFilter(request)
                .ApplyPagedDataAsync(request.PageNumber, request.PageSize, cancellationToken);
        }
    }
}
