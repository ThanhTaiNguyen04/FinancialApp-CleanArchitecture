using FinancialApp.Application.DTOs;
using FinancialApp.Application.Interfaces;
using FinancialApp.Domain.Entities;
using FinancialApp.Domain.Interfaces;

namespace FinancialApp.Application.Services;

public class BudgetService : IBudgetService
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITransactionRepository _transactionRepository;

    public BudgetService(IBudgetRepository budgetRepository, ICategoryRepository categoryRepository, ITransactionRepository transactionRepository)
    {
        _budgetRepository = budgetRepository;
        _categoryRepository = categoryRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<BudgetDto>> GetAllBudgetsAsync()
    {
        var budgets = await _budgetRepository.GetAllAsync();
        return budgets.Select(MapToBudgetDto);
    }

    public async Task<IEnumerable<BudgetDto>> GetUserBudgetsAsync(int userId)
    {
        var budgets = await _budgetRepository.GetByUserIdAsync(userId);
        return budgets.Select(MapToBudgetDto);
    }

    public async Task<IEnumerable<BudgetDto>> GetUserBudgetsByPeriodAsync(int userId, int month, int year)
    {
        var budgets = await _budgetRepository.GetByUserIdAndPeriodAsync(userId, month, year);
        return budgets.Select(MapToBudgetDto);
    }

    public async Task<BudgetDto?> GetBudgetByIdAsync(int id)
    {
        var budget = await _budgetRepository.GetByIdAsync(id);
        return budget != null ? MapToBudgetDto(budget) : null;
    }

    public async Task<BudgetSummaryDto> GetBudgetSummaryAsync(int userId, int month, int year)
    {
        var budgets = await _budgetRepository.GetByUserIdAndPeriodAsync(userId, month, year);
        var budgetDtos = budgets.Select(MapToBudgetDto).ToList();

        var totalBudget = budgetDtos.Sum(b => b.BudgetAmount);
        var totalSpent = budgetDtos.Sum(b => b.SpentAmount);

        return new BudgetSummaryDto
        {
            TotalBudget = totalBudget,
            TotalSpent = totalSpent,
            TotalRemaining = totalBudget - totalSpent,
            OverallPercentageUsed = totalBudget > 0 ? (totalSpent / totalBudget) * 100 : 0,
            Budgets = budgetDtos,
            ChartData = budgetDtos.Select(b => new BudgetCategoryChartDto
            {
                CategoryName = b.CategoryName,
                CategoryColor = b.CategoryColor,
                Amount = b.SpentAmount,
                Percentage = totalSpent > 0 ? (b.SpentAmount / totalSpent) * 100 : 0
            }).ToList()
        };
    }

    public async Task<BudgetDto> CreateBudgetAsync(int userId, CreateBudgetDto createBudgetDto)
    {
        // Check if budget already exists for this category and period
        var existingBudget = await _budgetRepository.GetByUserCategoryAndPeriodAsync(
            userId, createBudgetDto.CategoryId, createBudgetDto.Month, createBudgetDto.Year);
        
        if (existingBudget != null)
        {
            throw new InvalidOperationException("Budget already exists for this category and period");
        }

        // Calculate current spent amount for this category and period
        var transactions = await _transactionRepository.GetByUserIdAsync(userId);
        var category = await _categoryRepository.GetByIdAsync(createBudgetDto.CategoryId);
        
        var spentAmount = transactions
            .Where(t => t.Category == category?.Name && 
                       t.Type == "expense" &&
                       t.TransactionDate.Month == createBudgetDto.Month &&
                       t.TransactionDate.Year == createBudgetDto.Year)
            .Sum(t => t.Amount);

        var budget = new Budget
        {
            UserId = userId,
            CategoryId = createBudgetDto.CategoryId,
            BudgetAmount = createBudgetDto.BudgetAmount,
            SpentAmount = spentAmount,
            Month = createBudgetDto.Month,
            Year = createBudgetDto.Year
        };

        var createdBudget = await _budgetRepository.AddAsync(budget);
        return MapToBudgetDto(createdBudget);
    }

    public async Task<BudgetDto> UpdateBudgetAsync(int id, UpdateBudgetDto updateBudgetDto)
    {
        var budget = await _budgetRepository.GetByIdAsync(id);
        if (budget == null)
        {
            throw new ArgumentException("Budget not found");
        }

        budget.BudgetAmount = updateBudgetDto.BudgetAmount;
        var updatedBudget = await _budgetRepository.UpdateAsync(budget);
        return MapToBudgetDto(updatedBudget);
    }

    public async Task DeleteBudgetAsync(int id)
    {
        var budget = await _budgetRepository.GetByIdAsync(id);
        if (budget == null)
        {
            throw new ArgumentException("Budget not found");
        }

        await _budgetRepository.DeleteAsync(id);
    }

    public async Task UpdateSpentAmountAsync(int userId, string categoryName, decimal amount)
    {
        var category = await _categoryRepository.GetByNameAsync(categoryName);
        if (category == null) return;

        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        var budget = await _budgetRepository.GetByUserCategoryAndPeriodAsync(userId, category.Id, currentMonth, currentYear);
        if (budget != null)
        {
            var transactions = await _transactionRepository.GetByUserIdAsync(userId);
            var totalSpent = transactions
                .Where(t => t.Category == categoryName && 
                           t.Type == "expense" &&
                           t.TransactionDate.Month == currentMonth &&
                           t.TransactionDate.Year == currentYear)
                .Sum(t => t.Amount);

            budget.SpentAmount = totalSpent;
            await _budgetRepository.UpdateAsync(budget);
        }
    }

    private BudgetDto MapToBudgetDto(Budget budget)
    {
        return new BudgetDto
        {
            Id = budget.Id,
            UserId = budget.UserId,
            CategoryId = budget.CategoryId,
            CategoryName = budget.Category?.Name ?? "",
            CategoryIcon = budget.Category?.IconName ?? "",
            CategoryColor = budget.Category?.ColorCode ?? "",
            BudgetAmount = budget.BudgetAmount,
            SpentAmount = budget.SpentAmount,
            RemainingAmount = budget.RemainingAmount,
            PercentageUsed = budget.PercentageUsed,
            Month = budget.Month,
            Year = budget.Year,
            CreatedAt = budget.CreatedAt,
            UpdatedAt = budget.UpdatedAt
        };
    }
}