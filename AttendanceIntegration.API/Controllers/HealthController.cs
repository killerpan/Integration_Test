using Microsoft.AspNetCore.Mvc;

namespace AttendanceIntegration.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new 
        { 
            status = "Healthy", 
            timestamp = DateTime.UtcNow,
            service = "Attendance Integration API"
        });
    }
}
