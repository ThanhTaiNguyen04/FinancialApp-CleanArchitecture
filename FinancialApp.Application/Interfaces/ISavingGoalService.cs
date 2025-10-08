using FinancialApp.Application.DTOs;

namespace FinancialApp.Application.Interfaces;

public interface ISavingGoalService
{
    Task<IEnumerable<SavingGoalDto>> GetAllSavingGoalsAsync();
    Task<IEnumerable<SavingGoalDto>> GetUserSavingGoalsAsync(int userId);
    Task<IEnumerable<SavingGoalDto>> GetActiveSavingGoalsAsync(int userId);
    Task<SavingGoalDto?> GetSavingGoalByIdAsync(int id);
    Task<SavingGoalDto> CreateSavingGoalAsync(int userId, CreateSavingGoalDto createSavingGoalDto);
    Task<SavingGoalDto> UpdateSavingGoalAsync(int id, UpdateSavingGoalDto updateSavingGoalDto);
    Task<SavingGoalDto> AddToSavingGoalAsync(int id, AddToSavingGoalDto addToSavingGoalDto);
    Task DeleteSavingGoalAsync(int id);
}