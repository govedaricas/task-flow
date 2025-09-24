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

            var connectionString = "Server=DESKTOP-KQKTAKO\\SQLEXPRESS;Database=TaskFlow;User Id=sa;Password=Pa$$w0rd;TrustServerCertificate=True;";

            var builder = new DbContextOptionsBuilder<TaskFlowDbContext>();
            builder.UseSqlServer(connectionString);

            return new TaskFlowDbContext(builder.Options);
        }
    }
}
