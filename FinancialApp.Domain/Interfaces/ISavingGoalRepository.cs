using FinancialApp.Domain.Entities;

namespace FinancialApp.Domain.Interfaces;

public interface ISavingGoalRepository
{
    Task<IEnumerable<SavingGoal>> GetAllAsync();
    Task<IEnumerable<SavingGoal>> GetByUserIdAsync(int userId);
    Task<IEnumerable<SavingGoal>> GetActiveByUserIdAsync(int userId);
    Task<SavingGoal?> GetByIdAsync(int id);
    Task<SavingGoal> AddAsync(SavingGoal savingGoal);
    Task<SavingGoal> UpdateAsync(SavingGoal savingGoal);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}