using FinancialApp.Application.DTOs;

namespace FinancialApp.Application.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId);
    Task<IEnumerable<BudgetAlertDto>> GetBudgetAlertsAsync(int userId);
    Task<NotificationDto> CreateNotificationAsync(int userId, CreateNotificationDto createNotificationDto);
    Task MarkAsReadAsync(int notificationId);
    Task DeleteNotificationAsync(int notificationId);
}