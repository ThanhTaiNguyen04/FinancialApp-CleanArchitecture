namespace FinancialApp.Application.DTOs;

public class DashboardDto
{
    public UserDto User { get; set; } = new UserDto();
    public decimal AvailableBalance { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public decimal MonthlySpent { get; set; }
    public decimal SavingsProgress { get; set; }
    public List<TransactionDto> RecentTransactions { get; set; } = new List<TransactionDto>();
    public List<CategoryStatsDto> TopCategories { get; set; } = new List<CategoryStatsDto>();
    public BudgetSummaryDto BudgetOverview { get; set; } = new BudgetSummaryDto();
}

public class AdminStatsDto
{
    public int TotalUsers { get; set; }
    public int PremiumUsers { get; set; }
    public int FreeUsers { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalRevenue { get; set; }
    public int PendingPremiumRequests { get; set; }
}

public class FinancialChartDto
{
    public List<decimal> IncomeData { get; set; } = new List<decimal>();
    public List<decimal> ExpenseData { get; set; } = new List<decimal>();
    public List<string> Labels { get; set; } = new List<string>();
    public string Period { get; set; } = string.Empty; // "daily", "weekly", "monthly", "yearly"
}

public class AccountBalanceDto
{
    public decimal CurrentBalance { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public List<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
    public FinancialChartDto ChartData { get; set; } = new FinancialChartDto();
}