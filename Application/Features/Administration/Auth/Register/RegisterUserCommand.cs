using Application.Abstraction;

namespace Application.Features.Administration.Auth.Register
{
    public class RegisterUserCommand : IRequest<int>
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public bool IsActive { get; set; }
    }
}
