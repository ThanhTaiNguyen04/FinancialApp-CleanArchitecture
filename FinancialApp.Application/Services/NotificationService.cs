using FinancialApp.Application.DTOs;
using FinancialApp.Application.Interfaces;

namespace FinancialApp.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IBudgetService _budgetService;

    public NotificationService(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId)
    {
        var notifications = new List<NotificationDto>();
        
        // Get budget alerts as notifications
        var budgetAlerts = await GetBudgetAlertsAsync(userId);
        foreach (var alert in budgetAlerts)
        {
            notifications.Add(new NotificationDto
            {
                Id = alert.BudgetId,
                UserId = userId,
                Title = $"Cảnh báo ngân sách: {alert.CategoryName}",
                Message = alert.Message,
                Type = "budget_alert",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });
        }

        return notifications.OrderByDescending(n => n.CreatedAt);
    }

    public async Task<IEnumerable<BudgetAlertDto>> GetBudgetAlertsAsync(int userId)
    {
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;
        
        var budgets = await _budgetService.GetUserBudgetsByPeriodAsync(userId, currentMonth, currentYear);
        var alerts = new List<BudgetAlertDto>();

        foreach (var budget in budgets)
        {
            if (budget.PercentageUsed >= 80) // Alert when 80% or more is used
            {
                var alertLevel = budget.PercentageUsed switch
                {
                    >= 100 => "exceeded",
                    >= 95 => "danger",
                    >= 80 => "warning",
                    _ => "normal"
                };

                var message = alertLevel switch
                {
                    "exceeded" => $"⚠️ Bạn đã vượt quá {budget.PercentageUsed:F1}% ngân sách {budget.CategoryName}!",
                    "danger" => $"🚨 Bạn đã sử dụng {budget.PercentageUsed:F1}% ngân sách {budget.CategoryName}!",
                    "warning" => $"⚡ Bạn đã sử dụng {budget.PercentageUsed:F1}% ngân sách {budget.CategoryName}",
                    _ => ""
                };

                var colorCode = alertLevel switch
                {
                    "exceeded" => "#D32F2F", // Red
                    "danger" => "#FF5722",   // Orange-Red
                    "warning" => "#FF9800",  // Orange
                    _ => "#4CAF50"           // Green
                };

                if (!string.IsNullOrEmpty(message))
                {
                    alerts.Add(new BudgetAlertDto
                    {
                        BudgetId = budget.Id,
                        CategoryName = budget.CategoryName,
                        BudgetAmount = budget.BudgetAmount,
                        SpentAmount = budget.SpentAmount,
                        PercentageUsed = budget.PercentageUsed,
                        AlertLevel = alertLevel,
                        Message = message,
                        ColorCode = colorCode
                    });
                }
            }
        }

        return alerts.OrderByDescending(a => a.PercentageUsed);
    }

    public async Task<NotificationDto> CreateNotificationAsync(int userId, CreateNotificationDto createNotificationDto)
    {
        // In a real app, you would save this to database
        // For now, just return a mock notification
        return new NotificationDto
        {
            Id = new Random().Next(1000, 9999),
            UserId = userId,
            Title = createNotificationDto.Title,
            Message = createNotificationDto.Message,
            Type = createNotificationDto.Type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        // In a real app, you would update the notification in database
        await Task.CompletedTask;
    }

    public async Task DeleteNotificationAsync(int notificationId)
    {
        // In a real app, you would delete the notification from database
        await Task.CompletedTask;
    }
}