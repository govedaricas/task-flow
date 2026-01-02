namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required byte[] PasswordHash { get; set; }
        public byte[]? RefreshTokenHash { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public ICollection<Task> CreatedTasks { get; set; } = new List<Task>();
        public ICollection<Task> AssignedTasks { get; set; } = new List<Task>();
    }
}
