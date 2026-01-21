using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface ITaskFlowDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<Domain.Entities.Task> Tasks { get; set; }
        public DbSet<TaskPriority> TaskPriorities { get; set; }
        public DbSet<Domain.Entities.TaskStatus> TaskStatuses { get; set; }
        public DbSet<ProjectStatistics> ProjectStatistics { get; set; }

        public IUserIdentity? CurrentUser { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
