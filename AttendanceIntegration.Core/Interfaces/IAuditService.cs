using AttendanceIntegration.Core.Entities;

namespace AttendanceIntegration.Core.Interfaces;

public interface IAuditService
{
    Task LogAsync(string action, string entityType, string entityId, string userId, string details, bool success = true);
    Task<IEnumerable<AuditLog>> GetLogsAsync(DateTime? startDate = null, DateTime? endDate = null);
}
