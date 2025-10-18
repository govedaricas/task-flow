using Application.Interfaces;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
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

            services.AddDbContext<ITaskFlowDbContext, TaskFlowDbContext>(options =>
                options.UseNpgsql(connString));

            // Hangfire
            services.AddHangfire(config =>
            {
                config.UsePostgreSqlStorage(
                    connString,
                    new PostgreSqlStorageOptions
                    {
                        PrepareSchemaIfNecessary = false,
                        QueuePollInterval = TimeSpan.FromSeconds(15)
                    });
            });
            services.AddHangfireServer();

            return services;
        }
    }
}
