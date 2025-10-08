using Microsoft.AspNetCore.Mvc;
using FinancialApp.Application.DTOs;
using FinancialApp.Application.Interfaces;

namespace FinancialApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUserNotifications(int userId)
    {
        var notifications = await _notificationService.GetUserNotificationsAsync(userId);
        return Ok(notifications);
    }

    [HttpGet("user/{userId}/budget-alerts")]
    public async Task<ActionResult<IEnumerable<BudgetAlertDto>>> GetBudgetAlerts(int userId)
    {
        var alerts = await _notificationService.GetBudgetAlertsAsync(userId);
        return Ok(alerts);
    }

    [HttpPost("user/{userId}")]
    public async Task<ActionResult<NotificationDto>> CreateNotification(int userId, CreateNotificationDto createNotificationDto)
    {
        var notification = await _notificationService.CreateNotificationAsync(userId, createNotificationDto);
        return Ok(notification);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNotification(int id)
    {
        await _notificationService.DeleteNotificationAsync(id);
        return NoContent();
    }
}