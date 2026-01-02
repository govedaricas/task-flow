using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DotNetEnv;

namespace Persistance.Context
{
    internal class TaskFlowDbContextFactory : IDesignTimeDbContextFactory<TaskFlowDbContext>
    {
        public TaskFlowDbContext CreateDbContext(string[] args)
        {
            Env.Load();

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

            var apiProjectPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../task-flow-api"));

            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var connString = $"Host={host};Port={port};Database={db};Username={user};Password={pass}";

            var builder = new DbContextOptionsBuilder<TaskFlowDbContext>();
            builder.UseNpgsql(connString);

            return new TaskFlowDbContext(builder.Options);
        }
    }
}
