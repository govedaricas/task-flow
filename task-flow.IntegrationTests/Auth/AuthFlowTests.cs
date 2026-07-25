using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using task_flow.IntegrationTests.Collections;
using task_flow.IntegrationTests.Infrastructure;

namespace task_flow.IntegrationTests.Auth;

[Collection(IntegrationCollection.Name)]
public class AuthFlowTests : IntegrationTestBase
{
    public AuthFlowTests(TaskflowWebFactory factory) : base(factory) { }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsTokens()
    {
        // Arrange
        var request = new { Username = "admin", Password = "Admin123!" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(new JsonSerializerOptions(JsonSerializerDefaults.Web));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.GetProperty("success").GetBoolean().Should().BeTrue();
        body.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
        body.GetProperty("refreshToken").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WrongPassword_Returns409()
    {
        // Arrange
        var request = new { Username = "admin", Password = "WrongPassword!" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_NonExistentUser_Returns404()
    {
        // Arrange
        var request = new { Username = "ghost", Password = "any" };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Register_ValidData_ReturnsCreatedUserId()
    {
        // Arrange
        var request = new
        {
            Username = "newuser",
            Password = "NewPass123!",
            FirstName = "New",
            LastName = "User",
            Email = "newuser@test.com",
            IsActive = true
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(new JsonSerializerOptions(JsonSerializerDefaults.Web));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.GetInt32().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Register_DuplicateUsername_Returns409()
    {
        // Arrange — "admin" je već seedan
        var request = new
        {
            Username = "admin",
            Password = "SomePass123!",
            FirstName = "Dup",
            LastName = "User",
            Email = "dup@test.com",
            IsActive = true
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task RefreshToken_ValidToken_ReturnsNewTokenPair()
    {
        // Arrange — login pa uzmi refresh token
        var loginResp = await Client.PostAsJsonAsync("/api/auth/login", new { Username = "admin", Password = "Admin123!" });
        var loginBody = await loginResp.Content.ReadFromJsonAsync<JsonElement>(new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var refreshToken = loginBody.GetProperty("refreshToken").GetString()!;

        // Act
        var response = await Client.PostAsJsonAsync("/api/tokens/refresh", new { RefreshToken = refreshToken });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(new JsonSerializerOptions(JsonSerializerDefaults.Web));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
        body.GetProperty("refreshToken").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        // Act — nema Authorization headera
        var response = await Client.GetAsync("/api/tasks/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithExpiredToken_Returns401()
    {
        // Arrange — fabriciran JWT koji je već istekao
        var expiredToken = JwtTestHelper.GenerateExpiredToken();
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", expiredToken);

        // Act
        var response = await Client.GetAsync("/api/tasks/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
