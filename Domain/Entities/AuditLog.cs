namespace Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string EntityId { get; set; }
        public required string EntityName { get; set; }
        public required string Action { get; set; }
        public DateTime Timestamp { get; set; }
        public string? OldValues { get; set; } 
        public string? NewValues { get; set; } 
        public string? Changes { get; set; }
    }
}
