using AttendanceIntegration.Core.DTOs;
using AttendanceIntegration.Core.Interfaces;
using AttendanceIntegration.Infrastructure.ExternalServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceIntegration.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// Comentar [Authorize] para pruebas sin JWT
// [Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;
    private readonly MockApiClient _mockApiClient;
    private readonly ILogger<AttendanceController> _logger;

    public AttendanceController(
        IAttendanceService attendanceService, 
        MockApiClient mockApiClient,
        ILogger<AttendanceController> logger)
    {
        _attendanceService = attendanceService;
        _mockApiClient = mockApiClient;
        _logger = logger;
    }

    /// <summary>
    /// Importa automáticamente desde el Mock API externo
    /// </summary>
    [HttpPost("import-from-mock")]
    public async Task<ActionResult<AttendanceImportResponse>> ImportFromMockApi([FromQuery] int companyId)
    {
        _logger.LogInformation("Iniciando importación automática desde Mock API para empresa {CompanyId}", companyId);

        try
        {
            // Obtener datos del Mock API
            var mockData = await _mockApiClient.GetAttendanceFromMockAsync(companyId);

            if (mockData == null || mockData.Records == null || !mockData.Records.Any())
            {
                _logger.LogWarning("No se obtuvieron datos del Mock API para empresa {CompanyId}", companyId);
                return BadRequest(new AttendanceImportResponse
                {
                    Success = false,
                    Errors = new List<string> { "No se pudieron obtener datos del Mock API. Verifica que esté ejecutándose en https://localhost:7001" }
                });
            }

            // Importar los datos obtenidos
            var result = await _attendanceService.ImportAttendanceAsync(mockData);

            if (result.Success)
            {
                _logger.LogInformation("Importación automática exitosa: {Count} registros procesados", result.ProcessedRecords);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante la importación automática desde Mock API");
            return StatusCode(500, new AttendanceImportResponse
            {
                Success = false,
                Errors = new List<string> { $"Error interno: {ex.Message}" }
            });
        }
    }

    /// <summary>
    /// Importa registros de asistencia vía API (JSON manual)
    /// </summary>
    [HttpPost("import")]
    public async Task<ActionResult<AttendanceImportResponse>> ImportAttendance([FromBody] AttendanceImportRequest request)
    {
        _logger.LogInformation("Importing {Count} attendance records for company {CompanyId}", 
            request.Records.Count, request.CompanyId);

        var result = await _attendanceService.ImportAttendanceAsync(request);

        if (!result.Success)
        {
            _logger.LogWarning("Import failed: {Errors}", string.Join(", ", result.Errors));
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Importa registros de asistencia desde archivo Excel
    /// </summary>
    [HttpPost("import-excel")]
    public async Task<ActionResult<AttendanceImportResponse>> ImportFromExcel(IFormFile file, [FromQuery] int companyId)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided");

        if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            return BadRequest("File must be Excel format (.xlsx)");

        _logger.LogInformation("Importing Excel file {FileName} for company {CompanyId}", file.FileName, companyId);

        using var stream = file.OpenReadStream();
        var result = await _attendanceService.ImportFromExcelAsync(stream, companyId);

        if (!result.Success)
        {
            _logger.LogWarning("Excel import failed: {Errors}", string.Join(", ", result.Errors));
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene registros de asistencia por rango de fechas
    /// </summary>
    [HttpGet("{companyId}")]
    public async Task<ActionResult<IEnumerable<AttendanceRecordDto>>> GetAttendance(
        int companyId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        _logger.LogInformation("Retrieving attendance for company {CompanyId}", companyId);
        var records = await _attendanceService.GetAttendanceAsync(companyId, startDate, endDate);
        return Ok(records);
    }
}
