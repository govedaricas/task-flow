using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistance.Context
{
    internal class TaskFlowDbContextFactory : IDesignTimeDbContextFactory<TaskFlowDbContext>
    {
        public TaskFlowDbContext CreateDbContext(string[] args)
        {
            var apiProjectPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../task-flow-api"));

            var configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var connectionString = "Host=localhost;Port=5432;Database=TaskFlow;Username=postgres;Password=Pa$$w0rd";

            var builder = new DbContextOptionsBuilder<TaskFlowDbContext>();
            builder.UseNpgsql(connectionString);

            return new TaskFlowDbContext(builder.Options);
        }
    }
}
