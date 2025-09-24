using Application.Features.Auth.Login;
using Application.Features.Auth.Register;
using Microsoft.AspNetCore.Mvc;

namespace task_flow_api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly LoginCommandHandler _loginHandler;
        private readonly RegisterUserCommandHandler _registerHandler;

        public AuthController(LoginCommandHandler loginHandler, RegisterUserCommandHandler registerHandler)
        {
            _loginHandler = loginHandler;
            _registerHandler = registerHandler;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var responseModel = await _loginHandler.Handle(command, cancellationToken);

            if (responseModel == null)
                return Unauthorized(new { message = "Invalid credentials" });

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // true in production
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(2)
            };

            Response.Cookies.Append("token", responseModel.Token, cookieOptions);

            return Ok(new { responseModel });
        }

        [HttpPost("register")]
        public async Task<int> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
        {
            return await _registerHandler.Handle(command, cancellationToken);
        }
    }
}
