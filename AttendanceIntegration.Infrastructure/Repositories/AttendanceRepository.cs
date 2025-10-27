using AttendanceIntegration.Core.Entities;
using AttendanceIntegration.Core.Interfaces;
using AttendanceIntegration.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AttendanceIntegration.Infrastructure.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly ApplicationDbContext _context;

    public AttendanceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AttendanceRecord> AddAsync(AttendanceRecord record)
    {
        _context.AttendanceRecords.Add(record);
        await _context.SaveChangesAsync();
        return record;
    }

    public async Task<IEnumerable<AttendanceRecord>> AddRangeAsync(IEnumerable<AttendanceRecord> records)
    {
        _context.AttendanceRecords.AddRange(records);
        await _context.SaveChangesAsync();
        return records;
    }

    public async Task<IEnumerable<AttendanceRecord>> GetByCompanyAsync(int companyId, DateTime startDate, DateTime endDate)
    {
        return await _context.AttendanceRecords
            .Where(r => r.CompanyId == companyId && r.Date >= startDate && r.Date <= endDate)
            .OrderBy(r => r.Date)
            .ThenBy(r => r.EmployeeId)
            .ToListAsync();
    }

    public async Task<AttendanceRecord?> GetByIdAsync(int id)
    {
        return await _context.AttendanceRecords.FindAsync(id);
    }

    public async Task<bool> UpdateAsync(AttendanceRecord record)
    {
        _context.AttendanceRecords.Update(record);
        return await _context.SaveChangesAsync() > 0;
    }
}
