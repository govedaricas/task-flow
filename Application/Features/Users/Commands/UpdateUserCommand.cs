using Application.Abstraction;

namespace Application.Features.Users.Commands
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public bool IsActive { get; set; }
    }
}
