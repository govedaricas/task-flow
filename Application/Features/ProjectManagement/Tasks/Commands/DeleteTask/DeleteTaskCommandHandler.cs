using Application.Abstraction;
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

            _dbContext.Tasks.Remove(task);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
