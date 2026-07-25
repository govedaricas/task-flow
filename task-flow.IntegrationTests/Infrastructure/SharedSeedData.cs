namespace task_flow.IntegrationTests.Infrastructure;

public class SharedSeedData
{
    public int AdminUserId { get; set; }
    public int MemberUserId { get; set; }
    public int OutsiderUserId { get; set; }
    public int ProjectId { get; set; }
    public string AdminToken { get; set; } = string.Empty;
    public string MemberToken { get; set; } = string.Empty;
    public string OutsiderToken { get; set; } = string.Empty;
}
