using Application.Abstraction;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.ProjectManagement.Tasks.Commands.UpdateTask
{
    public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, bool>
    {
        private readonly ITaskFlowDbContext _dbContext;

        public UpdateTaskCommandHandler(ITaskFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _dbContext.Tasks
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (task == null)
            {
                throw new NotFoundException("Task", "Task not found.");
            }

            if (request.AssignedToUserId != task.AssignedUserId)
            {
                var userExists = await _dbContext.Users
                    .AnyAsync(x => x.Id == request.AssignedToUserId && x.IsActive);

                if (!userExists)
                {
                    throw new NotFoundException("Task", "Task not found.");
                }
            }

            task.Name = request.Name;
            task.AssignedUserId = request.AssignedToUserId;
            task.IsActive = request.IsActive;
            task.Description = request.Description;
            task.DueDate = request.DueDate;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
