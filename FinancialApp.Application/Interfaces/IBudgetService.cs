using FinancialApp.Application.DTOs;

namespace FinancialApp.Application.Interfaces;

public interface IBudgetService
{
    Task<IEnumerable<BudgetDto>> GetAllBudgetsAsync();
    Task<IEnumerable<BudgetDto>> GetUserBudgetsAsync(int userId);
    Task<IEnumerable<BudgetDto>> GetUserBudgetsByPeriodAsync(int userId, int month, int year);
    Task<BudgetDto?> GetBudgetByIdAsync(int id);
    Task<BudgetSummaryDto> GetBudgetSummaryAsync(int userId, int month, int year);
    Task<BudgetDto> CreateBudgetAsync(int userId, CreateBudgetDto createBudgetDto);
    Task<BudgetDto> UpdateBudgetAsync(int id, UpdateBudgetDto updateBudgetDto);
    Task DeleteBudgetAsync(int id);
    Task UpdateSpentAmountAsync(int userId, string category, decimal amount);
}