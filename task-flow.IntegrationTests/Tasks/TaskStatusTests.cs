using Application.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistance.Context;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using task_flow.IntegrationTests.Collections;
using task_flow.IntegrationTests.Infrastructure;

namespace task_flow.IntegrationTests.Tasks;

[Collection(IntegrationCollection.Name)]
public class TaskStatusTests : IntegrationTestBase
{
    public TaskStatusTests(TaskflowWebFactory factory) : base(factory) { }

    [Fact]
    public async Task ChangeStatus_NewToInProgress_UpdatesTaskInDb()
    {
        // Arrange — kreiraj task (default status = New)
        AuthorizeAs(Seed.AdminToken);
        var createResp = await Client.PostAsJsonAsync("/api/tasks", new { Name = "Status test", ProjectId = Seed.ProjectId, IsActive = true });
        var taskId = await createResp.Content.ReadFromJsonAsync<int>();

        // Act — promijeni status na InProgress
        var response = await Client.PutAsJsonAsync(
            $"/api/tasks/{taskId}/status",
            new { Id = taskId, TaskStatusId = (byte)TaskStatusEnum.InProgress });

        // Assert — HTTP
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert — baza
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
        var task = await db.Tasks.FindAsync(taskId);
        task!.TaskStatusId.Should().Be((byte)TaskStatusEnum.InProgress);
    }

    [Fact]
    public async Task ChangeStatus_NewToCompleted_StatisticsUpdated()
    {
        // Arrange
        AuthorizeAs(Seed.AdminToken);
        var createResp = await Client.PostAsJsonAsync("/api/tasks", new { Name = "Complete me", ProjectId = Seed.ProjectId, IsActive = true });
        var taskId = await createResp.Content.ReadFromJsonAsync<int>();

        // Uzmi statistike prije
        using var scopeBefore = Factory.Services.CreateScope();
        var dbBefore = scopeBefore.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
        var statsBefore = await dbBefore.ProjectStatistics.FirstAsync(x => x.ProjectId == Seed.ProjectId);
        var completedBefore = statsBefore.CompletedCount;

        // Act
        await Client.PutAsJsonAsync(
            $"/api/tasks/{taskId}/status",
            new { Id = taskId, TaskStatusId = (byte)TaskStatusEnum.Completed });

        // Assert — statistike su se promijenile
        using var scopeAfter = Factory.Services.CreateScope();
        var dbAfter = scopeAfter.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
        var statsAfter = await dbAfter.ProjectStatistics.FirstAsync(x => x.ProjectId == Seed.ProjectId);
        statsAfter.CompletedCount.Should().BeGreaterThan(completedBefore);
    }

    [Fact]
    public async Task ChangeStatus_AsOutsider_Returns401Or403()
    {
        // Arrange — kreiraj task kao admin
        AuthorizeAs(Seed.AdminToken);
        var createResp = await Client.PostAsJsonAsync("/api/tasks", new { Name = "Secure task", ProjectId = Seed.ProjectId, IsActive = true });
        var taskId = await createResp.Content.ReadFromJsonAsync<int>();

        // Act — outsider pokušava promijeniti status
        AuthorizeAs(Seed.OutsiderToken);
        var response = await Client.PutAsJsonAsync(
            $"/api/tasks/{taskId}/status",
            new { Id = taskId, TaskStatusId = (byte)TaskStatusEnum.Completed });

        // Assert — outsider nije member projekta, treba biti odbijen
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized);
    }
}
