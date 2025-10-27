namespace AttendanceIntegration.Core.Entities;

public class AttendanceRecord
{
    public int Id { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan? CheckIn { get; set; }
    public TimeSpan? CheckOut { get; set; }
    public decimal TotalHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public int CompanyId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Source { get; set; } = string.Empty;
    public bool Processed { get; set; }
}
