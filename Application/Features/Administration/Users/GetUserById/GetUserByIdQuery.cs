using Application.Abstraction;

namespace Application.Features.Administration.Users.GetUserById
{
    public class GetUserByIdQuery : IRequest<UserModel>
    {
        public int Id { get; set; }
    }
}
