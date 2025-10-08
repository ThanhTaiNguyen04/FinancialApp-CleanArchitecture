namespace FinancialApp.Application.DTOs;

public class NotificationDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "budget_alert", "goal_achieved", "info"
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateNotificationDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class BudgetAlertDto
{
    public int BudgetId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal BudgetAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal PercentageUsed { get; set; }
    public string AlertLevel { get; set; } = string.Empty; // "warning" (80%), "danger" (95%), "exceeded" (100%+)
    public string Message { get; set; } = string.Empty;
    public string ColorCode { get; set; } = string.Empty;
}