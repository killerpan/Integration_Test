using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AttendanceIntegration.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
    {
        if (!ValidateUser(request.Username, request.Password))
        {
            _logger.LogWarning("Login failed for: {Username}", request.Username);
            return Unauthorized(new { message = "Invalid credentials" });
        }

        var token = GenerateJwtToken(request.Username);

        return Ok(new LoginResponse
        {
            Token = token,
            ExpiresIn = 3600,
            TokenType = "Bearer",
            Username = request.Username
        });
    }

    [HttpGet("test-token")]
    public ActionResult<LoginResponse> GetTestToken()
    {
        var token = GenerateJwtToken("test-user");
        return Ok(new LoginResponse
        {
            Token = token,
            ExpiresIn = 3600,
            TokenType = "Bearer",
            Username = "test-user"
        });
    }

    private bool ValidateUser(string username, string password)
    {
        var validUsers = new Dictionary<string, string>
        {
            { "admin", "admin123" },
            { "user", "user123" },
            { "test", "test123" }
        };

        return validUsers.TryGetValue(username, out var validPassword) && 
               validPassword == password;
    }

    private string GenerateJwtToken(string username)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "default-secret-key-minimum-32-characters";
        var issuer = jwtSettings["Issuer"] ?? "AttendanceIntegrationAPI";
        var audience = jwtSettings["Audience"] ?? "AttendanceIntegrationClients";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim("company_id", "1")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public string TokenType { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}
