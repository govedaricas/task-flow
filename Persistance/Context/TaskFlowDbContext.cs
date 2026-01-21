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
        public DbSet<Role> Roles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectMember> ProjectMembers { get; set; }
        public DbSet<Domain.Entities.Task> Tasks { get; set; }
        public DbSet<TaskPriority> TaskPriorities { get; set; }
        public DbSet<Domain.Entities.TaskStatus> TaskStatuses { get; set; }
        public DbSet<ProjectStatistics> ProjectStatistics { get; set; }


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

                entity.HasMany(x => x.Roles).WithMany(x => x.Users)
                    .UsingEntity<Dictionary<string, object>>(
                        "UserRole",
                        j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                        j => j.HasOne<User>().WithMany().HasForeignKey("UserId"),
                        j =>
                        {
                            j.HasKey("UserId", "RoleId");
                            j.ToTable("UserRole", "Administration");
                        });
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

                entity.Property(e => e.OldValues).HasColumnType("text");
                entity.Property(e => e.NewValues).HasColumnType("text");
                entity.Property(e => e.Changes).HasColumnType("text");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role", "Administration");
                entity.HasKey(u => u.Id).HasName("PK_Role");

                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Project", "ProjectManagement");
                entity.HasKey(p => p.Id).HasName("PK_Project");

                entity.Property(p => p.Code).IsRequired().HasMaxLength(10);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Description).HasMaxLength(500);
                entity.Property(p => p.CreatedById).IsRequired();
                entity.Property(p => p.CreatedAt).IsRequired();
                entity.Property(p => p.IsActive).IsRequired();

                entity.HasOne(p => p.CreatedBy)
                    .WithMany()
                    .HasForeignKey(p => p.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Project_CreatedBy_User");

                entity.HasMany(p => p.Tasks)
                    .WithOne(t => t.Project)
                    .HasForeignKey(t => t.ProjectId)
                    .HasConstraintName("FK_Task_Project");

                entity.HasMany(p => p.Members)
                    .WithOne(m => m.Project)
                    .HasForeignKey(m => m.ProjectId)
                    .HasConstraintName("FK_ProjectMember_Project");

                entity.HasMany(p => p.ProjectStatistics)
                    .WithOne(m => m.Project)
                    .HasForeignKey(m => m.ProjectId)
                    .HasConstraintName("FK_ProjectStatistics_Project");
            });

            modelBuilder.Entity<ProjectMember>(entity =>
            {
                entity.ToTable("ProjectMember", "ProjectManagement");
                entity.HasKey(pm => pm.Id).HasName("PK_ProjectMember");

                entity.Property(pm => pm.ProjectId).IsRequired();
                entity.Property(pm => pm.UserId).IsRequired();

                entity.HasOne(pm => pm.Project)
                    .WithMany(p => p.Members)
                    .HasForeignKey(pm => pm.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ProjectMember_Project");

                entity.HasOne(pm => pm.User)
                    .WithMany(u => u.ProjectMembers)
                    .HasForeignKey(pm => pm.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ProjectMember_User");

                entity.HasIndex(pm => new { pm.ProjectId, pm.UserId }).IsUnique()
                    .HasDatabaseName("UQ_ProjectMember_ProjectId_UserId");
            });

            modelBuilder.Entity<Domain.Entities.Task>(entity =>
            {
                entity.ToTable("Task", "ProjectManagement");
                entity.HasKey(t => t.Id).HasName("PK_Task");

                entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Description).HasMaxLength(1000);
                entity.Property(t => t.ProjectId).IsRequired();
                entity.Property(t => t.TaskStatusId).IsRequired();
                entity.Property(t => t.TaskPriorityId).IsRequired();
                entity.Property(t => t.CreatedByUserId).IsRequired();
                entity.Property(t => t.CreatedAt).IsRequired();
                entity.Property(t => t.DueDate);
                entity.Property(t => t.IsActive).IsRequired();

                entity.HasOne(t => t.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(t => t.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Task_Project");

                entity.HasOne(t => t.CreatedByUser)
                    .WithMany(u => u.CreatedTasks)
                    .HasForeignKey(t => t.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Task_CreatedByUser");

                entity.HasOne(t => t.AssignedUser)
                    .WithMany(u => u.AssignedTasks)
                    .HasForeignKey(t => t.AssignedUserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Task_AssignedUser");

                entity.HasOne(t => t.TaskStatus)
                    .WithMany(ts => ts.Tasks)
                    .HasForeignKey(t => t.TaskStatusId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Task_TaskStatus");
            });

            modelBuilder.Entity<Domain.Entities.TaskStatus>(entity =>
            {
                entity.ToTable("TaskStatus", "BasicCatalog");
                entity.HasKey(ts => ts.Id).HasName("PK_TaskStatus");

                entity.Property(ts => ts.Name).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment", "ProjectManagement");
                entity.HasKey(c => c.Id).HasName("PK_Comment");

                entity.Property(c => c.Text).IsRequired().HasMaxLength(1000);
                entity.Property(c => c.CreatedAt).IsRequired();
                entity.Property(c => c.TaskId).IsRequired();
                entity.Property(c => c.AuthorId).IsRequired();

                entity.HasOne(c => c.Task)
                    .WithMany(t => t.Comments)
                    .HasForeignKey(c => c.TaskId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Comment_Task");

                entity.HasOne(c => c.Author)
                    .WithMany()
                    .HasForeignKey(c => c.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Comment_Author_User");
            });

            modelBuilder.Entity<ProjectStatistics>(entity =>
            {
                entity.ToTable("ProjectStatistics", "ProjectManagement");

                entity.HasKey(c => c.ProjectId).HasName("PK_ProjectStatistics");

                entity.HasOne(c => c.Project)
                    .WithMany(t => t.ProjectStatistics)
                    .HasForeignKey(c => c.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ProjectStatistics_Project");
            });

            modelBuilder.Entity<TaskPriority>(entity =>
            {
                entity.ToTable("TaskPriority", "BasicCatalog");
                entity.HasKey(tp => tp.Id).HasName("PK_TaskPriority");

                entity.Property(tp => tp.Name).IsRequired().HasMaxLength(50);
            });
        }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = CurrentUser?.Id.ToString() ?? "0";

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
