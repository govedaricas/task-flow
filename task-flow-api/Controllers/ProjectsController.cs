using Application.Features.ProjectManagement.Projects.Commands.AddPoject;
using Application.Features.ProjectManagement.Projects.Commands.DeleteProject;
using Application.Features.ProjectManagement.Projects.Commands.UpdateProject;
using Application.Features.ProjectManagement.Projects.Queries.GetAllProjects;
using Application.Features.ProjectManagement.Projects.Queries.GetProjectById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace task_flow_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly AddProjectCommandHandler _addProjectCommandHandler;
        private readonly UpdateProjectCommandHandler _updateProjectCommandHandler;
        private readonly GetProjectByIdQueryHandler _getProjectByIdQueryHandler;
        private readonly GetAllProjectsQueryHandler _getAllProjectsQueryHandler;
        private readonly DeleteProjectCommandHandler _deleteProjectCommandHandler;

        public ProjectsController(AddProjectCommandHandler addProjectCommandHandler, UpdateProjectCommandHandler updateProjectCommandHandler, GetProjectByIdQueryHandler getProjectByIdQueryHandler, GetAllProjectsQueryHandler getAllProjectsQueryHandler, DeleteProjectCommandHandler deleteProjectCommandHandler)
        {
            _addProjectCommandHandler = addProjectCommandHandler;
            _updateProjectCommandHandler = updateProjectCommandHandler;
            _getProjectByIdQueryHandler = getProjectByIdQueryHandler;
            _getAllProjectsQueryHandler = getAllProjectsQueryHandler;
            _deleteProjectCommandHandler = deleteProjectCommandHandler;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ProjectModel> GetProjectById(int id, CancellationToken cancellationToken)
        {
            return await _getProjectByIdQueryHandler.Handle(new GetProjectByIdQuery { Id = id }, cancellationToken);
        }

        [Authorize]
        [HttpGet]
        public async Task<List<ProjectModel>> GetAllProjects(CancellationToken cancellationToken)
        {
            return await _getAllProjectsQueryHandler.Handle(new GetAllProjectsQuery(), cancellationToken);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost]
        public async Task<int> CreateProject([FromBody] AddProjectCommand command, CancellationToken cancellationToken)
        {
            return await _addProjectCommandHandler.Handle(command, cancellationToken);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPut]
        public async Task<bool> UpdateProject([FromBody] UpdateProjectCommand command, CancellationToken cancellationToken)
        {
            return await _updateProjectCommandHandler.Handle(command, cancellationToken);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<bool> DeleteProject(int id, CancellationToken cancellationToken)
        {
            return await _deleteProjectCommandHandler.Handle(new DeleteProjectCommand { Id = id }, cancellationToken);
        }
    }
}
