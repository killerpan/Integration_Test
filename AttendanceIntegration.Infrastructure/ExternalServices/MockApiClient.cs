using AttendanceIntegration.Core.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AttendanceIntegration.Infrastructure.ExternalServices;

public class MockApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MockApiClient> _logger;
    private readonly string _baseUrl;

    public MockApiClient(HttpClient httpClient, IConfiguration configuration, ILogger<MockApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["MockApiSettings:BaseUrl"] ?? "https://localhost:7001";

        // Configurar HttpClient para aceptar certificados self-signed en desarrollo
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
    }

    public async Task<AttendanceImportRequest?> GetAttendanceFromMockAsync(int companyId)
    {
        try
        {
            var endpoint = $"{_baseUrl}/api/attendance/export/{companyId}";

            _logger.LogInformation("Conectando al Mock API: {Endpoint}", endpoint);

            var response = await _httpClient.GetAsync(endpoint);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error al obtener datos del Mock API. StatusCode: {StatusCode}", response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var data = JsonSerializer.Deserialize<AttendanceImportRequest>(content, options);

            if (data != null)
            {
                _logger.LogInformation("Obtenidos {Count} registros desde Mock API", data.Records?.Count ?? 0);
            }

            return data;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de conexión al Mock API. ¿Está ejecutándose en {BaseUrl}?", _baseUrl);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener datos del Mock API");
            return null;
        }
    }
}
