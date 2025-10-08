using FinancialApp.Application.DTOs;
using FinancialApp.Application.Interfaces;
using FinancialApp.Domain.Interfaces;

namespace FinancialApp.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUserService _userService;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBudgetService _budgetService;
    private readonly ISavingGoalRepository _savingGoalRepository;

    public DashboardService(
        IUserService userService,
        ITransactionRepository transactionRepository,
        IBudgetService budgetService,
        ISavingGoalRepository savingGoalRepository)
    {
        _userService = userService;
        _transactionRepository = transactionRepository;
        _budgetService = budgetService;
        _savingGoalRepository = savingGoalRepository;
    }

    public async Task<DashboardDto> GetDashboardDataAsync(int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        var transactions = await _transactionRepository.GetByUserIdAsync(userId);
        
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        // Calculate monthly income and expenses
        var monthlyTransactions = transactions.Where(t => 
            t.TransactionDate.Month == currentMonth && 
            t.TransactionDate.Year == currentYear).ToList();

        var monthlyIncome = monthlyTransactions.Where(t => t.Type == "income").Sum(t => t.Amount);
        var monthlyExpenses = monthlyTransactions.Where(t => t.Type == "expense").Sum(t => t.Amount);

        // Get recent transactions (last 10)
        var recentTransactions = transactions
            .OrderByDescending(t => t.TransactionDate)
            .Take(10)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Type = t.Type,
                Category = t.Category,
                Amount = t.Amount,
                Description = t.Description,
                IconName = t.IconName,
                TransactionDate = t.TransactionDate,
                CreatedAt = t.CreatedAt
            }).ToList();

        // Get top spending categories
        var topCategories = monthlyTransactions
            .Where(t => t.Type == "expense")
            .GroupBy(t => t.Category)
            .Select(g => new CategoryStatsDto
            {
                Category = g.Key,
                Amount = g.Sum(t => t.Amount),
                Percentage = monthlyExpenses > 0 ? (g.Sum(t => t.Amount) / monthlyExpenses) * 100 : 0
            })
            .OrderByDescending(c => c.Amount)
            .Take(5)
            .ToList();

        // Get budget overview
        var budgetOverview = await _budgetService.GetBudgetSummaryAsync(userId, currentMonth, currentYear);

        // Calculate savings progress
        var savingGoals = await _savingGoalRepository.GetActiveByUserIdAsync(userId);
        var totalSavingsTarget = savingGoals.Sum(s => s.TargetAmount);
        var totalSavingsCurrent = savingGoals.Sum(s => s.CurrentAmount);
        var savingsProgress = totalSavingsTarget > 0 ? (totalSavingsCurrent / totalSavingsTarget) * 100 : 0;

        return new DashboardDto
        {
            User = user ?? new UserDto(),
            AvailableBalance = user?.AvailableBalance ?? 0,
            MonthlyIncome = monthlyIncome,
            MonthlyExpenses = monthlyExpenses,
            MonthlySpent = monthlyExpenses,
            SavingsProgress = savingsProgress,
            RecentTransactions = recentTransactions,
            TopCategories = topCategories,
            BudgetOverview = budgetOverview
        };
    }

    public async Task<AccountBalanceDto> GetAccountBalanceAsync(int userId, string period = "monthly")
    {
        var user = await _userService.GetUserByIdAsync(userId);
        var transactions = await _transactionRepository.GetByUserIdAsync(userId);

        var totalIncome = transactions.Where(t => t.Type == "income").Sum(t => t.Amount);
        var totalExpenses = transactions.Where(t => t.Type == "expense").Sum(t => t.Amount);

        // Filter transactions based on period
        var filteredTransactions = FilterTransactionsByPeriod(transactions, period);

        var transactionDtos = filteredTransactions
            .OrderByDescending(t => t.TransactionDate)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                UserId = t.UserId,
                Type = t.Type,
                Category = t.Category,
                Amount = t.Amount,
                Description = t.Description,
                IconName = t.IconName,
                TransactionDate = t.TransactionDate,
                CreatedAt = t.CreatedAt
            }).ToList();

        var chartData = await GetFinancialChartDataAsync(userId, period);

        return new AccountBalanceDto
        {
            CurrentBalance = user?.AvailableBalance ?? 0,
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            Transactions = transactionDtos,
            ChartData = chartData
        };
    }

    public async Task<FinancialChartDto> GetFinancialChartDataAsync(int userId, string period = "daily", int days = 30)
    {
        var transactions = await _transactionRepository.GetByUserIdAsync(userId);
        var labels = new List<string>();
        var incomeData = new List<decimal>();
        var expenseData = new List<decimal>();

        switch (period.ToLower())
        {
            case "daily":
                for (int i = days - 1; i >= 0; i--)
                {
                    var date = DateTime.UtcNow.AddDays(-i);
                    var dayTransactions = transactions.Where(t => t.TransactionDate.Date == date.Date).ToList();
                    
                    labels.Add(date.ToString("dd/MM"));
                    incomeData.Add(dayTransactions.Where(t => t.Type == "income").Sum(t => t.Amount));
                    expenseData.Add(dayTransactions.Where(t => t.Type == "expense").Sum(t => t.Amount));
                }
                break;

            case "weekly":
                for (int i = 11; i >= 0; i--)
                {
                    var weekStart = DateTime.UtcNow.AddDays(-(DateTime.UtcNow.DayOfWeek - DayOfWeek.Monday)).AddDays(-i * 7);
                    var weekEnd = weekStart.AddDays(6);
                    var weekTransactions = transactions.Where(t => t.TransactionDate.Date >= weekStart.Date && t.TransactionDate.Date <= weekEnd.Date).ToList();
                    
                    labels.Add($"T{i + 1}");
                    incomeData.Add(weekTransactions.Where(t => t.Type == "income").Sum(t => t.Amount));
                    expenseData.Add(weekTransactions.Where(t => t.Type == "expense").Sum(t => t.Amount));
                }
                break;

            case "monthly":
                for (int i = 11; i >= 0; i--)
                {
                    var monthDate = DateTime.UtcNow.AddMonths(-i);
                    var monthTransactions = transactions.Where(t => 
                        t.TransactionDate.Month == monthDate.Month && 
                        t.TransactionDate.Year == monthDate.Year).ToList();
                    
                    labels.Add(monthDate.ToString("MM/yyyy"));
                    incomeData.Add(monthTransactions.Where(t => t.Type == "income").Sum(t => t.Amount));
                    expenseData.Add(monthTransactions.Where(t => t.Type == "expense").Sum(t => t.Amount));
                }
                break;

            case "yearly":
                for (int i = 4; i >= 0; i--)
                {
                    var year = DateTime.UtcNow.Year - i;
                    var yearTransactions = transactions.Where(t => t.TransactionDate.Year == year).ToList();
                    
                    labels.Add(year.ToString());
                    incomeData.Add(yearTransactions.Where(t => t.Type == "income").Sum(t => t.Amount));
                    expenseData.Add(yearTransactions.Where(t => t.Type == "expense").Sum(t => t.Amount));
                }
                break;
        }

        return new FinancialChartDto
        {
            IncomeData = incomeData,
            ExpenseData = expenseData,
            Labels = labels,
            Period = period
        };
    }

    private IEnumerable<Domain.Entities.Transaction> FilterTransactionsByPeriod(IEnumerable<Domain.Entities.Transaction> transactions, string period)
    {
        var now = DateTime.UtcNow;
        
        return period.ToLower() switch
        {
            "daily" => transactions.Where(t => t.TransactionDate.Date == now.Date),
            "weekly" => transactions.Where(t => t.TransactionDate >= now.AddDays(-7)),
            "monthly" => transactions.Where(t => t.TransactionDate.Month == now.Month && t.TransactionDate.Year == now.Year),
            "yearly" => transactions.Where(t => t.TransactionDate.Year == now.Year),
            _ => transactions
        };
    }
}