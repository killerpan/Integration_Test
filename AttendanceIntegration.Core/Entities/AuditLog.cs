namespace AttendanceIntegration.Core.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
    public bool Success { get; set; }
}
