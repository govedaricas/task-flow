using Application.Features.ProjectManagement.Tasks.Commands.AddTask;
using Application.Features.ProjectManagement.Tasks.Commands.ChangeTaskStatus;
using Application.Features.ProjectManagement.Tasks.Commands.DeleteTask;
using Application.Features.ProjectManagement.Tasks.Commands.UpdateTask;
using Application.Features.ProjectManagement.Tasks.Queries.GetAllTasks;
using Application.Features.ProjectManagement.Tasks.Queries.GetTaskById;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        private readonly ChangeTaskStatusCommandHandler _changeTaskStatusCommandHandler;
        private readonly ITaskNotificationService _notificationService;
        private readonly ITaskFlowDbContext _dbContext;

        public TasksController(AddTaskCommandHandler addTaskCommandHandler, UpdateTaskCommandHandler updateTaskCommandHandler, GetTaskByIdQueryHandler getTaskByIdQueryHandler, GetAllTasksQueryHandler getAllTasksQueryHandler, DeleteTaskCommandHandler deleteTaskCommandHandler, ChangeTaskStatusCommandHandler changeTaskStatusCommandHandler, ITaskNotificationService notificationService, ITaskFlowDbContext dbContext)
        {
            _addTaskCommandHandler = addTaskCommandHandler;
            _updateTaskCommandHandler = updateTaskCommandHandler;
            _getTaskByIdQueryHandler = getTaskByIdQueryHandler;
            _getAllTasksQueryHandler = getAllTasksQueryHandler;
            _deleteTaskCommandHandler = deleteTaskCommandHandler;
            _changeTaskStatusCommandHandler = changeTaskStatusCommandHandler;
            _notificationService = notificationService;
            _dbContext = dbContext;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<TaskModel> GetTaskById(int id, CancellationToken cancellationToken)
        {
            return await _getTaskByIdQueryHandler.Handle(new GetTaskByIdQuery { Id = id }, cancellationToken);
        }

        [HttpGet]
        [Authorize]
        public async Task<List<TaskModel>> GetAllTasks(CancellationToken cancellationToken)
        {
            return await _getAllTasksQueryHandler.Handle(new GetAllTasksQuery(), cancellationToken);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,TaskManager")]
        public async Task<int> CreateTask([FromBody] AddTaskCommand command, CancellationToken cancellationToken)
        {
            return await _addTaskCommandHandler.Handle(command, cancellationToken);
        }

        [HttpPut]
        [Authorize(Roles = "Admin,TaskManager")]
        public async Task<bool> UpdateTask([FromBody] UpdateTaskCommand command, CancellationToken cancellationToken)
        {
            return await _updateTaskCommandHandler.Handle(command, cancellationToken);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<bool> DeleteTask(int id, CancellationToken cancellationToken)
        {
            return await _deleteTaskCommandHandler.Handle(new DeleteTaskCommand { Id = id }, cancellationToken);
        }

        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> ChangeTaskStatus(int id, [FromBody] ChangeTaskStatusCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            var task = await _changeTaskStatusCommandHandler.Handle(command, cancellationToken);

            var userIds = await _dbContext.ProjectMembers
                .Where(x => x.ProjectId == task.ProjectId)
                .Select(x => x.UserId)
                .ToListAsync(cancellationToken);

            await _notificationService.NotifyTaskStatusChanged(id, command.TaskStatusId.ToString(), task.Name, userIds);

            return Ok(task);
        }
    }
}
