namespace FinancialApp.Application.DTOs;

public class SavingGoalDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal PercentageCompleted { get; set; }
    public DateTime TargetDate { get; set; }
    public string IconName { get; set; } = string.Empty;
    public string ColorCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int DaysRemaining { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateSavingGoalDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public DateTime TargetDate { get; set; }
    public string IconName { get; set; } = string.Empty;
    public string ColorCode { get; set; } = string.Empty;
}

public class UpdateSavingGoalDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? TargetAmount { get; set; }
    public DateTime? TargetDate { get; set; }
    public string? IconName { get; set; }
    public string? ColorCode { get; set; }
    public string? Status { get; set; }
}

public class AddToSavingGoalDto
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}