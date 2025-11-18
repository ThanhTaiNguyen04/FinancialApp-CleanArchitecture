using Microsoft.AspNetCore.Mvc;

namespace FinancialApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Health check endpoint for keep-alive and monitoring
    /// </summary>
    [HttpGet]
    public ActionResult<object> Get()
    {
        return Ok(new { 
            status = "healthy", 
            timestamp = DateTime.UtcNow,
            service = "FinancialApp API"
        });
    }
}