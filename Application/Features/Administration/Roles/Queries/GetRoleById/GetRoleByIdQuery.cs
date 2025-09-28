using Application.Abstraction;

namespace Application.Features.Administration.Roles.Queries.GetRoleById
{
    public class GetRoleByIdQuery : IRequest<RoleModel>
    {
        public int Id { get; set; }
    }
}
