namespace AttendanceIntegration.Core.DTOs;

public class AttendanceRecordDto
{
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan? CheckIn { get; set; }
    public TimeSpan? CheckOut { get; set; }
    public decimal TotalHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public int CompanyId { get; set; }
}

public class AttendanceImportRequest
{
    public int CompanyId { get; set; }
    public List<AttendanceRecordDto> Records { get; set; } = new();
}

public class AttendanceImportResponse
{
    public bool Success { get; set; }
    public int TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public int FailedRecords { get; set; }
    public List<string> Errors { get; set; } = new();
}
