using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Persistance.Context;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using task_flow.IntegrationTests.Collections;
using task_flow.IntegrationTests.Infrastructure;

namespace task_flow.IntegrationTests.Tasks;

[Collection(IntegrationCollection.Name)]
public class TaskCrudTests : IntegrationTestBase
{
    public TaskCrudTests(TaskflowWebFactory factory) : base(factory) { }

    // ── CREATE ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateTask_AsAdmin_Returns201AndTaskExistsInDb()
    {
        // Arrange
        AuthorizeAs(Seed.AdminToken);
        var request = new { Name = "Implement login", ProjectId = Seed.ProjectId, IsActive = true };

        // Act
        var response = await Client.PostAsJsonAsync("/api/tasks", request);
        var taskId = await response.Content.ReadFromJsonAsync<int>();

        // Assert — HTTP
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        taskId.Should().BeGreaterThan(0);

        // Assert — baza
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
        var task = await db.Tasks.FindAsync(taskId);
        task.Should().NotBeNull();
        task!.Name.Should().Be("Implement login");
        task.ProjectId.Should().Be(Seed.ProjectId);
    }

    [Fact]
    public async Task CreateTask_WithoutToken_Returns401()
    {
        // Arrange — bez Authorization headera
        var request = new { Name = "Task", ProjectId = Seed.ProjectId, IsActive = true };

        // Act
        var response = await Client.PostAsJsonAsync("/api/tasks", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTask_AsMemberWithoutRole_Returns403()
    {
        // Arrange — member nema Admin/TaskManager rolu
        AuthorizeAs(Seed.MemberToken);
        var request = new { Name = "Task", ProjectId = Seed.ProjectId, IsActive = true };

        // Act
        var response = await Client.PostAsJsonAsync("/api/tasks", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateTask_NonExistentProject_Returns404()
    {
        // Arrange
        AuthorizeAs(Seed.AdminToken);
        var request = new { Name = "Task", ProjectId = 99999, IsActive = true };

        // Act
        var response = await Client.PostAsJsonAsync("/api/tasks", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── GET ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetTask_ExistingTask_Returns200WithData()
    {
        // Arrange — kreiraj task pa ga dohvati
        AuthorizeAs(Seed.AdminToken);
        var createResp = await Client.PostAsJsonAsync("/api/tasks", new { Name = "Read me", ProjectId = Seed.ProjectId, IsActive = true });
        var taskId = await createResp.Content.ReadFromJsonAsync<int>();

        // Act
        var response = await Client.GetAsync($"/api/tasks/{taskId}");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(new JsonSerializerOptions(JsonSerializerDefaults.Web));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.GetProperty("name").GetString().Should().Be("Read me");
    }

    [Fact]
    public async Task GetTask_NonExistent_Returns404()
    {
        // Arrange
        AuthorizeAs(Seed.AdminToken);

        // Act
        var response = await Client.GetAsync("/api/tasks/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ── DELETE ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteTask_AsAdmin_Returns200AndRemovedFromDb()
    {
        // Arrange — kreiraj task
        AuthorizeAs(Seed.AdminToken);
        var createResp = await Client.PostAsJsonAsync("/api/tasks", new { Name = "Delete me", ProjectId = Seed.ProjectId, IsActive = true });
        var taskId = await createResp.Content.ReadFromJsonAsync<int>();

        // Act
        var response = await Client.DeleteAsync($"/api/tasks/{taskId}");

        // Assert — HTTP
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert — baza
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
        var task = await db.Tasks.FindAsync(taskId);
        task.Should().BeNull();
    }

    [Fact]
    public async Task DeleteTask_AsMember_Returns403()
    {
        // Arrange — admin kreira task, member pokušava obrisati
        AuthorizeAs(Seed.AdminToken);
        var createResp = await Client.PostAsJsonAsync("/api/tasks", new { Name = "Protected", ProjectId = Seed.ProjectId, IsActive = true });
        var taskId = await createResp.Content.ReadFromJsonAsync<int>();

        AuthorizeAs(Seed.MemberToken);

        // Act
        var response = await Client.DeleteAsync($"/api/tasks/{taskId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
