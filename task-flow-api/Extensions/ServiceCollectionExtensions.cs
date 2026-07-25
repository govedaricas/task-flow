using Application.Interfaces;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Persistance.Context;

namespace task_flow_api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            var host = Environment.GetEnvironmentVariable("POSTGRES_HOST")
                ?? throw new InvalidOperationException("POSTGRES_HOST not set in environment");
            var port = Environment.GetEnvironmentVariable("POSTGRES_PORT")
                ?? throw new InvalidOperationException("POSTGRES_PORT not set in environment");
            var db = Environment.GetEnvironmentVariable("POSTGRES_DB")
                ?? throw new InvalidOperationException("POSTGRES_DB not set in environment");
            var user = Environment.GetEnvironmentVariable("POSTGRES_USER")
                ?? throw new InvalidOperationException("POSTGRES_USER not set in environment");
            var pass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")
                ?? throw new InvalidOperationException("POSTGRES_PASSWORD not set in environment");

            var connString = $"Host={host};Port={port};Database={db};Username={user};Password={pass}";

            var redisConfig = Environment.GetEnvironmentVariable("REDIS_CONFIGURATION")
                ?? config["Redis:Configuration"]
                ?? throw new InvalidOperationException("REDIS_CONFIGURATION not set in environment");

            services.AddHealthChecks()
                .AddNpgSql(
                    connectionString: connString,
                    name: "postgresql",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["ready"])
                .AddRedis(
                    redisConnectionString: redisConfig,
                    name: "redis",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["ready"]);

            services.AddDbContext<TaskFlowDbContext>(options =>
                options.UseNpgsql(connString));

            services.AddScoped<ITaskFlowDbContext>(sp => sp.GetRequiredService<TaskFlowDbContext>());

            // Hangfire
            services.AddHangfire(config =>
            {
                config.UsePostgreSqlStorage(
                    connString,
                    new PostgreSqlStorageOptions
                    {
                        PrepareSchemaIfNecessary = true,
                        QueuePollInterval = TimeSpan.FromSeconds(15)
                    });
            });
            services.AddHangfireServer();

            return services;
        }
    }
}
