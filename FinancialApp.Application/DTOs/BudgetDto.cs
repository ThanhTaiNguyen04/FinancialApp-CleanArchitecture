namespace FinancialApp.Application.DTOs;

public class BudgetDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryIcon { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = string.Empty;
    public decimal BudgetAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal PercentageUsed { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateBudgetDto
{
    public int CategoryId { get; set; }
    public decimal BudgetAmount { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}

public class UpdateBudgetDto
{
    public decimal BudgetAmount { get; set; }
}

public class BudgetSummaryDto
{
    public decimal TotalBudget { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal TotalRemaining { get; set; }
    public decimal OverallPercentageUsed { get; set; }
    public List<BudgetDto> Budgets { get; set; } = new List<BudgetDto>();
    public List<BudgetCategoryChartDto> ChartData { get; set; } = new List<BudgetCategoryChartDto>();
}

public class BudgetCategoryChartDto
{
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryColor { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}