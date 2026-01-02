using Application.Features.ProjectManagement.Tasks.Commands.AddTask;
using Application.Features.ProjectManagement.Tasks.Commands.DeleteTask;
using Application.Features.ProjectManagement.Tasks.Commands.UpdateTask;
using Application.Features.ProjectManagement.Tasks.Queries.GetAllTasks;
using Application.Features.ProjectManagement.Tasks.Queries.GetTaskById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace task_flow_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly AddTaskCommandHandler _addTaskCommandHandler;
        private readonly UpdateTaskCommandHandler _updateTaskCommandHandler;
        private readonly GetTaskByIdQueryHandler _getTaskByIdQueryHandler;
        private readonly GetAllTasksQueryHandler _getAllTasksQueryHandler;
        private readonly DeleteTaskCommandHandler _deleteTaskCommandHandler;

        public TasksController(AddTaskCommandHandler addTaskCommandHandler, UpdateTaskCommandHandler updateTaskCommandHandler, GetTaskByIdQueryHandler getTaskByIdQueryHandler, GetAllTasksQueryHandler getAllTasksQueryHandler, DeleteTaskCommandHandler deleteTaskCommandHandler)
        {
            _addTaskCommandHandler = addTaskCommandHandler;
            _updateTaskCommandHandler = updateTaskCommandHandler;
            _getTaskByIdQueryHandler = getTaskByIdQueryHandler;
            _getAllTasksQueryHandler = getAllTasksQueryHandler;
            _deleteTaskCommandHandler = deleteTaskCommandHandler;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<TaskModel> GetTaskById(int id, CancellationToken cancellationToken)
        {
            return await _getTaskByIdQueryHandler.Handle(new GetTaskByIdQuery { Id = id }, cancellationToken);
        }

        [Authorize]
        [HttpGet]
        public async Task<List<TaskModel>> GetAllTasks(CancellationToken cancellationToken)
        {
            return await _getAllTasksQueryHandler.Handle(new GetAllTasksQuery(), cancellationToken);
        }

        [Authorize(Roles = "Admin,TaskManager")]
        [HttpPost]
        public async Task<int> CreateTask([FromBody] AddTaskCommand command, CancellationToken cancellationToken)
        {
            return await _addTaskCommandHandler.Handle(command, cancellationToken);
        }

        [Authorize(Roles = "Admin,TaskManager")]
        [HttpPut]
        public async Task<bool> UpdateTask([FromBody] UpdateTaskCommand command, CancellationToken cancellationToken)
        {
            return await _updateTaskCommandHandler.Handle(command, cancellationToken);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<bool> DeleteTask(int id, CancellationToken cancellationToken)
        {
            return await _deleteTaskCommandHandler.Handle(new DeleteTaskCommand { Id = id }, cancellationToken);
        }
    }
}
