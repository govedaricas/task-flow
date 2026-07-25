using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Persistance.Context;
using System.Net.Http.Json;
using System.Text.Json;
using Task = System.Threading.Tasks.Task;

namespace task_flow.IntegrationTests.Infrastructure;

public static class SeedHelper
{
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public static async Task<SharedSeedData> SeedCommonDataAsync(IServiceProvider services, HttpClient client)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        // --- Lookup data (idempotent) ---
        await EnsureRolesAsync(db);
        await EnsureTaskStatusesAsync(db);
        await EnsureTaskPrioritiesAsync(db);

        // --- Users ---
        var adminUser = await CreateUserAsync(db, hasher, "admin", "Admin", "User", "admin@test.com", "Admin123!", roleId: 1);
        var memberUser = await CreateUserAsync(db, hasher, "member", "Member", "User", "member@test.com", "Member123!", roleId: null);
        var outsiderUser = await CreateUserAsync(db, hasher, "outsider", "Out", "Sider", "outsider@test.com", "Out123!", roleId: null);

        // --- Project ---
        var project = new Project
        {
            Code = "TEST01",
            Name = "Test Project",
            CreatedById = adminUser.Id,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        db.Projects.Add(project);

        // --- Project statistics ---
        var stats = new ProjectStatistics { ProjectId = project.Id };

        await db.SaveChangesAsync();

        stats.ProjectId = project.Id;
        db.ProjectStatistics.Add(stats);

        // --- Project members: admin + member (outsider is NOT a member) ---
        db.ProjectMembers.Add(new ProjectMember { ProjectId = project.Id, UserId = adminUser.Id });
        db.ProjectMembers.Add(new ProjectMember { ProjectId = project.Id, UserId = memberUser.Id });

        await db.SaveChangesAsync();

        // --- Get tokens via login API ---
        var adminToken = await LoginAsync(client, "admin", "Admin123!");
        var memberToken = await LoginAsync(client, "member", "Member123!");
        var outsiderToken = await LoginAsync(client, "outsider", "Out123!");

        return new SharedSeedData
        {
            AdminUserId = adminUser.Id,
            MemberUserId = memberUser.Id,
            OutsiderUserId = outsiderUser.Id,
            ProjectId = project.Id,
            AdminToken = adminToken,
            MemberToken = memberToken,
            OutsiderToken = outsiderToken
        };
    }

    public static async Task<string> LoginAsync(HttpClient client, string username, string password)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { Username = username, Password = password });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(JsonOpts);
        return body.GetProperty("token").GetString()
            ?? throw new InvalidOperationException("Login did not return a token.");
    }

    private static async Task<User> CreateUserAsync(
        TaskFlowDbContext db,
        IPasswordHasher hasher,
        string username,
        string firstName,
        string lastName,
        string email,
        string password,
        int? roleId)
    {
        var passwordHash = Convert.FromBase64String(hasher.Hash(password));
        var user = new User
        {
            Username = username,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PasswordHash = passwordHash,
            IsActive = true
        };

        if (roleId.HasValue)
        {
            var role = await db.Roles.FindAsync(roleId.Value)
                ?? throw new InvalidOperationException($"Role {roleId} not found.");
            user.Roles.Add(role);
        }

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    private static async Task EnsureRolesAsync(TaskFlowDbContext db)
    {
        if (db.Roles.Any()) return;
        db.Roles.AddRange(
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "TaskManager" },
            new Role { Id = 3, Name = "Member" }
        );
        await db.SaveChangesAsync();
    }

    private static async Task EnsureTaskStatusesAsync(TaskFlowDbContext db)
    {
        if (db.TaskStatuses.Any()) return;
        db.TaskStatuses.AddRange(
            new Domain.Entities.TaskStatus { Id = 1, Name = "New" },
            new Domain.Entities.TaskStatus { Id = 2, Name = "InProgress" },
            new Domain.Entities.TaskStatus { Id = 3, Name = "OnHold" },
            new Domain.Entities.TaskStatus { Id = 4, Name = "Completed" },
            new Domain.Entities.TaskStatus { Id = 5, Name = "Cancelled" }
        );
        await db.SaveChangesAsync();
    }

    private static async Task EnsureTaskPrioritiesAsync(TaskFlowDbContext db)
    {
        if (db.TaskPriorities.Any()) return;
        db.TaskPriorities.AddRange(
            new TaskPriority { Id = 1, Name = "Low" },
            new TaskPriority { Id = 2, Name = "Medium" },
            new TaskPriority { Id = 3, Name = "High" },
            new TaskPriority { Id = 4, Name = "Critical" }
        );
        await db.SaveChangesAsync();
    }
}
