using Application.Features.ProjectManagement.Comments.Commands.AddComment;
using Application.Features.ProjectManagement.Comments.Commands.DeleteComment;
using Application.Features.ProjectManagement.Comments.Commands.UpdateComment;
using Application.Features.ProjectManagement.Comments.Queries.GetAllComments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace task_flow_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly AddCommentCommandHandler _addCommentCommandHandler;
        private readonly UpdateCommentCommandHandler _updateCommentCommandHandler;
        private readonly DeleteCommentCommandHandler _deleteCommentCommandHandler;
        private readonly GetAllCommentsQueryHandler _getAllCommentsQueryHandler;

        public CommentsController(
            AddCommentCommandHandler addCommentCommandHandler,
            UpdateCommentCommandHandler updateCommentCommandHandler,
            DeleteCommentCommandHandler deleteCommentCommandHandler,
            GetAllCommentsQueryHandler getAllCommentsQueryHandler)
        {
            _addCommentCommandHandler = addCommentCommandHandler;
            _updateCommentCommandHandler = updateCommentCommandHandler;
            _deleteCommentCommandHandler = deleteCommentCommandHandler;
            _getAllCommentsQueryHandler = getAllCommentsQueryHandler;
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpGet]
        public async Task<List<CommentModel>> GetAll([FromQuery] int taskId, CancellationToken cancellationToken)
        {
            return await _getAllCommentsQueryHandler.Handle(new GetAllCommentsQuery { TaskId = taskId }, cancellationToken);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost]
        public async Task<int> AddComment([FromBody] AddCommentCommand command, CancellationToken cancellationToken)
        {
            return await _addCommentCommandHandler.Handle(command, cancellationToken);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPut]
        public async Task<bool> UpdateComment([FromBody] UpdateCommentCommand command, CancellationToken cancellationToken)
        {
            return await _updateCommentCommandHandler.Handle(command, cancellationToken);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpDelete("{id}")]
        public async Task<bool> DeleteComment(int id, CancellationToken cancellationToken)
        {
            return await _deleteCommentCommandHandler.Handle(new DeleteCommentCommand { Id = id }, cancellationToken);
        }
    }
}
