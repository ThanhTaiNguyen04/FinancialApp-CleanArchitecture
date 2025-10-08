using FinancialApp.Domain.Entities;

namespace FinancialApp.Domain.Interfaces;

public interface IBudgetRepository
{
    Task<IEnumerable<Budget>> GetAllAsync();
    Task<IEnumerable<Budget>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Budget>> GetByUserIdAndPeriodAsync(int userId, int month, int year);
    Task<Budget?> GetByIdAsync(int id);
    Task<Budget?> GetByUserCategoryAndPeriodAsync(int userId, int categoryId, int month, int year);
    Task<Budget> AddAsync(Budget budget);
    Task<Budget> UpdateAsync(Budget budget);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}