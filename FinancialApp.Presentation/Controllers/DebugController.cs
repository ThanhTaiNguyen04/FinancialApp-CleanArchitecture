using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinancialApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DebugController : ControllerBase
{
    private readonly ILogger<DebugController> _logger;

    public DebugController(ILogger<DebugController> logger)
    {
        _logger = logger;
    }

    [HttpGet("token-info")]
    public ActionResult GetTokenInfo()
    {
        _logger.LogInformation("Debug: Token info requested");
        
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        var customUserId = User.FindFirst("UserId")?.Value;
        
        var tokenInfo = new
        {
            UserId = userId,
            CustomUserId = customUserId,
            AllClaims = claims,
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
            AuthenticationType = User.Identity?.AuthenticationType,
            Name = User.Identity?.Name
        };

        _logger.LogInformation("Debug: Current user ID from token: {UserId}, Custom UserId: {CustomUserId}", userId, customUserId);
        
        return Ok(tokenInfo);
    }

    [HttpGet("user-transactions")]
    public ActionResult GetCurrentUserInfo()
    {
        var currentUserId = GetCurrentUserId();
        _logger.LogInformation("Debug: Current user ID: {UserId}", currentUserId);
        
        return Ok(new { CurrentUserId = currentUserId, Message = "This is your user ID from JWT token" });
    }
    
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            _logger.LogError("Unable to extract user ID from JWT token");
            throw new UnauthorizedAccessException("Invalid or missing user ID in token");
        }
        return userId;
    }
}