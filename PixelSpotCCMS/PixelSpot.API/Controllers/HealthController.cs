using Microsoft.AspNetCore.Mvc;

namespace PixelSpot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { status = "healthy", message = "API is running", timestamp = DateTime.UtcNow });
    }
}