using System.Net.Http.Headers;

namespace task_flow.IntegrationTests.Infrastructure;

/// <summary>
/// Base klasa za testove koji mijenjaju stanje baze.
/// Svaki test dobija čistu bazu sa zajedničkim seedom.
/// </summary>
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly TaskflowWebFactory Factory;
    protected HttpClient Client = null!;

    protected SharedSeedData Seed => Factory.Seed;

    protected IntegrationTestBase(TaskflowWebFactory factory)
    {
        Factory = factory;
    }

    public async Task InitializeAsync()
    {
        await Factory.ResetAndSeedAsync();
        Client = Factory.CreateClient();
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    protected void AuthorizeAs(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
