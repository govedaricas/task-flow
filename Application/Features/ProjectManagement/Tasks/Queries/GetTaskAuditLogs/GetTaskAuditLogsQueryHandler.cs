using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Tasks.Queries.GetTaskAuditLogs
{
    public class GetTaskAuditLogsQueryHandler : IRequestHandler<GetTaskAuditLogsQuery, List<TaskAuditModel>>
    {
        private readonly ITaskFlowDbContext _dbContext;

        public GetTaskAuditLogsQueryHandler(ITaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<TaskAuditModel>> Handle(GetTaskAuditLogsQuery request, CancellationToken cancellationToken)
        {
            // Verify task exists
            var exists = await _dbContext.Tasks.AsNoTracking().AnyAsync(t => t.Id == request.TaskId, cancellationToken);
            if (!exists)
                throw new NotFoundException("Task", "Task not found.");

            var entityId = request.TaskId.ToString();

            var query = from a in _dbContext.AuditLogs.AsNoTracking()
                        where a.EntityName == nameof(Domain.Entities.Task) && a.EntityId == entityId
                        join u in _dbContext.Users.AsNoTracking() on a.UserId equals u.Id into uj
                        from u in uj.DefaultIfEmpty()
                        orderby a.Timestamp descending
                        select new TaskAuditModel
                        {
                            Id = a.Id,
                            UserId = a.UserId,
                            UserName = u != null ? (u.FirstName + " " + u.LastName) : null,
                            EntityId = a.EntityId,
                            EntityName = a.EntityName,
                            Action = a.Action,
                            Timestamp = a.Timestamp,
                            OldValues = a.OldValues,
                            NewValues = a.NewValues,
                            Changes = a.Changes
                        };

            return await query.ToListAsync(cancellationToken);
        }
    }
}
