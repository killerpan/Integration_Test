using AttendanceIntegration.Core.DTOs;
using AttendanceIntegration.Core.Entities;
using AttendanceIntegration.Core.Interfaces;

namespace AttendanceIntegration.Core.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _repository;
    private readonly IAuditService _auditService;

    public AttendanceService(IAttendanceRepository repository, IAuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
    }

    public async Task<AttendanceImportResponse> ImportAttendanceAsync(AttendanceImportRequest request)
    {
        var response = new AttendanceImportResponse { TotalRecords = request.Records.Count };

        try
        {
            var records = request.Records.Select(dto => new AttendanceRecord
            {
                EmployeeId = dto.EmployeeId,
                EmployeeName = dto.EmployeeName,
                Date = dto.Date,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                TotalHours = dto.TotalHours,
                OvertimeHours = dto.OvertimeHours,
                CompanyId = dto.CompanyId,
                CreatedAt = DateTime.UtcNow,
                Source = "API",
                Processed = false
            }).ToList();

            await _repository.AddRangeAsync(records);

            response.ProcessedRecords = records.Count;
            response.Success = true;

            await _auditService.LogAsync("ImportAttendance", "AttendanceRecord", 
                request.CompanyId.ToString(), "System", 
                $"Imported {records.Count} records via API", true);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
            response.FailedRecords = response.TotalRecords - response.ProcessedRecords;

            await _auditService.LogAsync("ImportAttendance", "AttendanceRecord",
                request.CompanyId.ToString(), "System", $"Failed: {ex.Message}", false);
        }

        return response;
    }

    public async Task<AttendanceImportResponse> ImportFromExcelAsync(List<AttendanceRecordDto> records)
    {
        var response = new AttendanceImportResponse { TotalRecords = records.Count };

        try
        {
            var entities = records.Select(dto => new AttendanceRecord
            {
                EmployeeId = dto.EmployeeId,
                EmployeeName = dto.EmployeeName,
                Date = dto.Date,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                TotalHours = dto.TotalHours,
                OvertimeHours = dto.OvertimeHours,
                CompanyId = dto.CompanyId,
                CreatedAt = DateTime.UtcNow,
                Source = "Excel",
                Processed = false
            }).ToList();

            await _repository.AddRangeAsync(entities);

            response.ProcessedRecords = entities.Count;
            response.Success = true;

            await _auditService.LogAsync("ImportFromExcel", "AttendanceRecord",
                entities.FirstOrDefault()?.CompanyId.ToString() ?? "0",
                "System", $"Imported {entities.Count} records via Excel", true);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Errors.Add(ex.Message);
            response.FailedRecords = response.TotalRecords - response.ProcessedRecords;

            await _auditService.LogAsync("ImportFromExcel", "AttendanceRecord",
                records.FirstOrDefault()?.CompanyId.ToString() ?? "0",
                "System", $"Failed: {ex.Message}", false);
        }

        return response;
    }


    public async Task<IEnumerable<AttendanceRecordDto>> GetAttendanceAsync(int companyId, DateTime startDate, DateTime endDate)
    {
        var records = await _repository.GetByCompanyAsync(companyId, startDate, endDate);

        return records.Select(r => new AttendanceRecordDto
        {
            EmployeeId = r.EmployeeId,
            EmployeeName = r.EmployeeName,
            Date = r.Date,
            CheckIn = r.CheckIn,
            CheckOut = r.CheckOut,
            TotalHours = r.TotalHours,
            OvertimeHours = r.OvertimeHours,
            CompanyId = r.CompanyId
        });
    }
}
