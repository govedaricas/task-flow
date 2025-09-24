using Application.Abstraction;

namespace Application.Features.Users.GetUserById
{
    public class GetUserByIdQuery : IRequest<UserModel>
    {
        public int Id { get; set; }
    }
}
