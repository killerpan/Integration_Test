using AttendanceIntegration.Core.Entities;
using OfficeOpenXml;

namespace AttendanceIntegration.Infrastructure.ExternalServices;

public class ExcelParserService
{
    public async Task<List<AttendanceRecord>> ParseAttendanceExcelAsync(Stream fileStream, int companyId)
    {
        var records = new List<AttendanceRecord>();
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage(fileStream);
        var worksheet = package.Workbook.Worksheets[0];

        if (worksheet == null)
            throw new Exception("No worksheet found");

        var rowCount = worksheet.Dimension?.Rows ?? 0;

        for (int row = 2; row <= rowCount; row++)
        {
            try
            {
                var record = new AttendanceRecord
                {
                    EmployeeId = worksheet.Cells[row, 1].Text,
                    EmployeeName = worksheet.Cells[row, 2].Text,
                    Date = DateTime.Parse(worksheet.Cells[row, 3].Text),
                    CheckIn = !string.IsNullOrEmpty(worksheet.Cells[row, 4].Text) 
                        ? TimeSpan.Parse(worksheet.Cells[row, 4].Text) : null,
                    CheckOut = !string.IsNullOrEmpty(worksheet.Cells[row, 5].Text) 
                        ? TimeSpan.Parse(worksheet.Cells[row, 5].Text) : null,
                    TotalHours = decimal.Parse(worksheet.Cells[row, 6].Text),
                    OvertimeHours = decimal.Parse(worksheet.Cells[row, 7].Text),
                    CompanyId = companyId,
                    CreatedAt = DateTime.UtcNow,
                    Source = "Excel",
                    Processed = false
                };

                records.Add(record);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error row {row}: {ex.Message}");
            }
        }

        return records;
    }
}
