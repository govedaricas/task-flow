using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using task_flow.IntegrationTests.Collections;
using task_flow.IntegrationTests.Infrastructure;

namespace task_flow.IntegrationTests.Validation;

[Collection(IntegrationCollection.Name)]
public class TaskValidationTests : IntegrationTestBase
{
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public TaskValidationTests(TaskflowWebFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateTask_EmptyName_Returns400WithErrorMessage()
    {
        // Arrange
        AuthorizeAs(Seed.AdminToken);
        var request = new { Name = "", ProjectId = Seed.ProjectId, IsActive = true };

        // Act
        var response = await Client.PostAsJsonAsync("/api/tasks", request);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("Name"); // FluentValidation poruka sadrži naziv polja
    }

    [Fact]
    public async Task CreateTask_NameExceedsMaxLength_Returns400()
    {
        // Arrange — max je 100 znakova
        AuthorizeAs(Seed.AdminToken);
        var request = new { Name = new string('x', 101), ProjectId = Seed.ProjectId, IsActive = true };

        // Act
        var response = await Client.PostAsJsonAsync("/api/tasks", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTask_ProjectIdZero_Returns400()
    {
        // Arrange
        AuthorizeAs(Seed.AdminToken);
        var request = new { Name = "Valid name", ProjectId = 0, IsActive = true };

        // Act
        var response = await Client.PostAsJsonAsync("/api/tasks", request);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("ProjectId");
    }

    [Fact]
    public async Task CreateTask_DueDateInPast_Returns400()
    {
        // Arrange
        AuthorizeAs(Seed.AdminToken);
        var request = new
        {
            Name = "Past due",
            ProjectId = Seed.ProjectId,
            IsActive = true,
            DueDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/tasks", request);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("DueDate");
    }

    [Fact]
    public async Task CreateTask_DescriptionTooLong_Returns400()
    {
        // Arrange — max je 1000 znakova
        AuthorizeAs(Seed.AdminToken);
        var request = new
        {
            Name = "Valid name",
            ProjectId = Seed.ProjectId,
            IsActive = true,
            Description = new string('x', 1001)
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/tasks", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_ShortPassword_Returns400()
    {
        // Arrange — min je 8 znakova
        var request = new
        {
            Username = "testval",
            Password = "Ab1!",
            FirstName = "Test",
            LastName = "Val",
            Email = "testval@test.com",
            IsActive = true
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("Password");
    }

    [Fact]
    public async Task Register_InvalidEmail_Returns400()
    {
        // Arrange
        var request = new
        {
            Username = "testval2",
            Password = "ValidPass123!",
            FirstName = "Test",
            LastName = "Val",
            Email = "not-an-email",
            IsActive = true
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);
        var body = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("Email");
    }
}
