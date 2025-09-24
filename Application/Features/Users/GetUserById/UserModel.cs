namespace Application.Features.Users.GetUserById
{
    public class UserModel
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public bool IsActive { get; set; }
    }
}
