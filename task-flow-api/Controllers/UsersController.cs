using Application.Features.Administration.Users.Commands;
using Application.Features.Administration.Users.GetUserById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace task_flow_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly GetUserByIdQueryHandler _getUserHandler;
        private readonly UpdateUserCommandHandler _updateUserCommandHandler;

        public UsersController(GetUserByIdQueryHandler getUserHandler, UpdateUserCommandHandler updateUserCommandHandler)
        {
            _getUserHandler = getUserHandler;
            _updateUserCommandHandler = updateUserCommandHandler;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<UserModel> GetUserById(int id, CancellationToken cancellationToken)
        {
            var result = await _getUserHandler.Handle(new GetUserByIdQuery { Id = id}, cancellationToken);
            return result;
        }

        [Authorize(Roles = "Admin")]
        [HttpPut()]
        public async Task<bool> UpdateUser([FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
        {
            return await _updateUserCommandHandler.Handle(command, cancellationToken);
        }
    }
}
