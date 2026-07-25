using Application.Features.ProjectManagement.Comments.Commands.AddComment;
using Application.Enums;
using Application.Features.ProjectManagement.Comments.Commands.DeleteComment;
using Application.Features.ProjectManagement.Comments.Commands.UpdateComment;
using Application.Features.ProjectManagement.Comments.Queries.GetAllComments;
using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        private readonly ITaskNotificationService _notificationService;

        public CommentsController(
            AddCommentCommandHandler addCommentCommandHandler,
            UpdateCommentCommandHandler updateCommentCommandHandler,
            DeleteCommentCommandHandler deleteCommentCommandHandler,
            GetAllCommentsQueryHandler getAllCommentsQueryHandler,
            ITaskNotificationService notificationService)
        {
            _addCommentCommandHandler = addCommentCommandHandler;
            _updateCommentCommandHandler = updateCommentCommandHandler;
            _deleteCommentCommandHandler = deleteCommentCommandHandler;
            _getAllCommentsQueryHandler = getAllCommentsQueryHandler;
            _notificationService = notificationService;
        }

        [HttpPost("search")]
        [Authorize(Roles = nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.ProjectManager))]
        public async Task<PagedData<CommentModel>> GetAll([FromQuery] GetAllCommentsQuery query, CancellationToken cancellationToken)
        {
            return await _getAllCommentsQueryHandler.Handle(query, cancellationToken);
        }

        [HttpPost]
        [Authorize(Roles = nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.ProjectManager))]
        public async Task<int> AddComment([FromBody] AddCommentCommand command, CancellationToken cancellationToken)
        {
            var comment = await _addCommentCommandHandler.Handle(command, cancellationToken);
            var authorName = comment.Author.FirstName + " " + comment.Author.LastName;

            await _notificationService.NotifyTaskCommentAdded(comment.TaskId, comment.Text, authorName, comment.AuthorId);

            return comment.Id;
        }

        [HttpPut]
        [Authorize(Roles = nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.ProjectManager))]
        public async Task<bool> UpdateComment([FromBody] UpdateCommentCommand command, CancellationToken cancellationToken)
        {
            return await _updateCommentCommandHandler.Handle(command, cancellationToken);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(RoleEnum.Admin) + "," + nameof(RoleEnum.ProjectManager))]
        public async Task<bool> DeleteComment(int id, CancellationToken cancellationToken)
        {
            return await _deleteCommentCommandHandler.Handle(new DeleteCommentCommand { Id = id }, cancellationToken);
        }
    }
}
