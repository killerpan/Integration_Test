using AttendanceIntegration.Core.Entities;
using AttendanceIntegration.Core.Interfaces;
using AttendanceIntegration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AttendanceIntegration.Infrastructure.Repositories;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;

    public AuditService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(string action, string entityType, string entityId, string userId, string details, bool success = true)
    {
        var log = new AuditLog
        {
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            UserId = userId,
            Timestamp = DateTime.UtcNow,
            Details = details,
            Success = success
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetLogsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.AuditLogs.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(l => l.Timestamp >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(l => l.Timestamp <= endDate.Value);

        return await query.OrderByDescending(l => l.Timestamp).ToListAsync();
    }
}
