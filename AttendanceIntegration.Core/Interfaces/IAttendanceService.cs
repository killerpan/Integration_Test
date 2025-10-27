using AttendanceIntegration.Core.DTOs;

namespace AttendanceIntegration.Core.Interfaces;

public interface IAttendanceService
{
    Task<AttendanceImportResponse> ImportAttendanceAsync(AttendanceImportRequest request);
    Task<AttendanceImportResponse> ImportFromExcelAsync(Stream fileStream, int companyId);
    Task<IEnumerable<AttendanceRecordDto>> GetAttendanceAsync(int companyId, DateTime startDate, DateTime endDate);
}
