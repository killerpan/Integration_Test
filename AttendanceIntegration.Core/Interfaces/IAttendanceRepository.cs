using AttendanceIntegration.Core.Entities;

namespace AttendanceIntegration.Core.Interfaces;

public interface IAttendanceRepository
{
    Task<AttendanceRecord> AddAsync(AttendanceRecord record);
    Task<IEnumerable<AttendanceRecord>> AddRangeAsync(IEnumerable<AttendanceRecord> records);
    Task<IEnumerable<AttendanceRecord>> GetByCompanyAsync(int companyId, DateTime startDate, DateTime endDate);
    Task<AttendanceRecord?> GetByIdAsync(int id);
    Task<bool> UpdateAsync(AttendanceRecord record);
}
