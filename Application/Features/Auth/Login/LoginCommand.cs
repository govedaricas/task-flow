using Application.Abstraction;

namespace Application.Features.Auth.Login
{
    public class LoginCommand : IRequest<LoginResponseModel>
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
