using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface ITaskFlowDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public IUserIdentity? CurrentUser { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
