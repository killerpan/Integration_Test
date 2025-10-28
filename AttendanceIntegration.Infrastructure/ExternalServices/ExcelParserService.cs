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
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet == null)
            throw new Exception("El archivo Excel no contiene hojas de cálculo válidas.");

        var rowCount = worksheet.Dimension?.Rows ?? 0;
        for (int row = rowCount; row >= 2; row--)
        {
            try
            {
                var record = new AttendanceRecord
                {
                    EmployeeId = worksheet.Cells[row, 1].Text.Trim(),
                    EmployeeName = worksheet.Cells[row, 2].Text.Trim(),
                    Date = DateTime.Parse(worksheet.Cells[row, 3].Text),
                    CheckIn = TimeSpan.TryParse(worksheet.Cells[row, 4].Text, out var ci) ? ci : (TimeSpan?)null,
                    CheckOut = TimeSpan.TryParse(worksheet.Cells[row, 5].Text, out var co) ? co : (TimeSpan?)null,
                    TotalHours = decimal.TryParse(worksheet.Cells[row, 6].Text, out var t) ? t : 0,
                    OvertimeHours = decimal.TryParse(worksheet.Cells[row, 7].Text, out var ot) ? ot : 0,
                    CompanyId = companyId,
                    CreatedAt = DateTime.UtcNow,
                    Source = "Excel",
                    Processed = false
                };
                records.Add(record);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en fila {row}: {ex.Message}");
                Console.WriteLine(ex);
            }
        }

        return records;
    }
}
