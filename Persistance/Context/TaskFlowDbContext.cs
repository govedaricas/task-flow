using System.Text.Json;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Persistance.Context
{
    public class TaskFlowDbContext : DbContext, ITaskFlowDbContext
    {
        public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        public IUserIdentity? CurrentUser { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "Administration");
                entity.HasKey(u => u.Id).HasName("PK_User");

                entity.Property(x => x.Username).IsRequired().HasMaxLength(10);
                entity.Property(x => x.FirstName).IsRequired().HasMaxLength(20);
                entity.Property(x => x.LastName).IsRequired().HasMaxLength(20);
                entity.Property(x => x.Email).IsRequired().HasMaxLength(100);
                entity.Property(x => x.PasswordHash).IsRequired();
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLog", "Administration");
                entity.HasKey(u => u.Id).HasName("PK_AuditLog");

                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.EntityId).IsRequired().HasMaxLength(12);
                entity.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.OldValues).HasColumnType("nvarchar(max)");
                entity.Property(e => e.NewValues).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Changes).HasColumnType("nvarchar(max)");
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = CurrentUser?.Id?.ToString() ?? "0";

            var modifiedEntities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                         || e.State == EntityState.Modified
                         || e.State == EntityState.Deleted)
                .ToList();

            foreach (var modifiedEntity in modifiedEntities)
            {
                var auditLog = new AuditLog
                {
                    UserId = int.Parse(userId),
                    EntityId = GetEntityId(modifiedEntity),
                    EntityName = modifiedEntity.Entity.GetType().Name,
                    Action = modifiedEntity.State.ToString(),
                    Timestamp = DateTime.UtcNow,
                    OldValues = GetChanges(modifiedEntity, oldOnly: true),
                    NewValues = GetChanges(modifiedEntity, newOnly: true),
                    Changes = GetChanges(modifiedEntity)
                };

                AuditLogs.Add(auditLog);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private string GetEntityId(EntityEntry entry)
        {
            var key = entry.Metadata.FindPrimaryKey();
            if (key != null)
            {
                var values = key.Properties.Select(p => entry.Property(p.Name).CurrentValue);
                return string.Join(",", values);
            }
            return "N/A";
        }

        private static string GetChanges(EntityEntry entity, bool oldOnly = false, bool newOnly = false)
        {
            var changes = new Dictionary<string, object?>();

            foreach (var property in entity.OriginalValues.Properties)
            {
                var originalValue = entity.OriginalValues[property];
                var currentValue = entity.CurrentValues[property];

                if (!Equals(originalValue, currentValue))
                {
                    if (oldOnly)
                        changes[property.Name] = originalValue;
                    else if (newOnly)
                        changes[property.Name] = currentValue;
                    else
                        changes[property.Name] = new { Old = originalValue, New = currentValue };
                }
            }

            return JsonSerializer.Serialize(changes);
        }
    }
}
