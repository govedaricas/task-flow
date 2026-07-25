using task_flow.IntegrationTests.Infrastructure;

namespace task_flow.IntegrationTests.Collections;

/// <summary>
/// Svi testovi koji dijele isti Docker container (PostgreSQL + Redis).
/// Jedan container se pokreće za cijelu kolekciju — reset se radi između testova u IntegrationTestBase.
/// </summary>
[CollectionDefinition(Name)]
public class IntegrationCollection : ICollectionFixture<TaskflowWebFactory>
{
    public const string Name = "Integration";
}
