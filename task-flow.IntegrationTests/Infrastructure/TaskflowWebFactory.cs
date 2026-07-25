using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Persistance.Context;
using Respawn;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace task_flow.IntegrationTests.Infrastructure;

public class TaskflowWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("taskflow_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly RedisContainer _redis = new RedisBuilder()
        .WithImage("redis:7-alpine")
        .Build();

    private Respawner _respawner = null!;
    private NpgsqlConnection _respawnConnection = null!;

    public SharedSeedData Seed { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_postgres.StartAsync(), _redis.StartAsync());

        // Postavi env varijable PRIJE nego factory kreira host (factory je lazy)
        Environment.SetEnvironmentVariable("POSTGRES_HOST", _postgres.Hostname);
        Environment.SetEnvironmentVariable("POSTGRES_PORT", _postgres.GetMappedPublicPort(5432).ToString());
        Environment.SetEnvironmentVariable("POSTGRES_DB", "taskflow_test");
        Environment.SetEnvironmentVariable("POSTGRES_USER", "postgres");
        Environment.SetEnvironmentVariable("POSTGRES_PASSWORD", "postgres");
        Environment.SetEnvironmentVariable("REDIS_CONFIGURATION", _redis.GetConnectionString());
        Environment.SetEnvironmentVariable("JWT_ISSUER", "taskflow-test");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "taskflow-test-client");
        Environment.SetEnvironmentVariable("JWT_SECRETKEY", "super-secret-key-for-integration-tests-minimum32chars!");
        Environment.SetEnvironmentVariable("SMTP_SENDEREMAIL", "test@test.com");
        Environment.SetEnvironmentVariable("SMTP_USERNAME", "test");
        Environment.SetEnvironmentVariable("SMTP_PASSWORD", "test");

        // Prisili factory da izgradi host i pokrene migracije
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
        await db.Database.MigrateAsync();

        // Pripremi Respawn konekciju
        _respawnConnection = new NpgsqlConnection(_postgres.GetConnectionString());
        await _respawnConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_respawnConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["Administration", "ProjectManagement", "BasicCatalog"]
        });

        // Inicijalni seed
        await ResetAndSeedAsync();
    }

    /// <summary>
    /// Briše sve redove iz app shema i ponovo seeda zajednički dataset.
    /// Poziva se na početku svakog testa koji mijenja stanje.
    /// </summary>
    public async Task ResetAndSeedAsync()
    {
        await _respawner.ResetAsync(_respawnConnection);
        Seed = await SeedHelper.SeedCommonDataAsync(Services, CreateClient());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // Zamijeni email servis sa no-op (nema slanja emailova u testovima)
            services.RemoveAll<IEmailService>();
            services.AddSingleton<IEmailService, NoOpEmailService>();

            // Zamijeni SignalR notifikacije sa no-op (nema WebSocket konekcija u testovima)
            services.RemoveAll<ITaskNotificationService>();
            services.AddScoped<ITaskNotificationService, NoOpTaskNotificationService>();
        });
    }

    public new async Task DisposeAsync()
    {
        await _respawnConnection.DisposeAsync();
        await _postgres.DisposeAsync();
        await _redis.DisposeAsync();
        await base.DisposeAsync();
    }
}
