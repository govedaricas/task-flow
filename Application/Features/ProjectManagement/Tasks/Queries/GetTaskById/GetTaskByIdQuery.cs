using Application.Abstraction;
using Application.Features.ProjectManagement.Tasks.Queries.GetAllTasks;

namespace Application.Features.ProjectManagement.Tasks.Queries.GetTaskById
{
    public class GetTaskByIdQuery : IRequest<TaskModel>
    {
        public int Id { get; set; }
    }
}
