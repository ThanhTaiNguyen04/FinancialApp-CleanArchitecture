using FinancialApp.Application.DTOs;
using FinancialApp.Application.Interfaces;
using FinancialApp.Domain.Entities;
using FinancialApp.Domain.Interfaces;

namespace FinancialApp.Application.Services;

public class SavingGoalService : ISavingGoalService
{
    private readonly ISavingGoalRepository _savingGoalRepository;

    public SavingGoalService(ISavingGoalRepository savingGoalRepository)
    {
        _savingGoalRepository = savingGoalRepository;
    }

    public async Task<IEnumerable<SavingGoalDto>> GetAllSavingGoalsAsync()
    {
        var savingGoals = await _savingGoalRepository.GetAllAsync();
        return savingGoals.Select(MapToSavingGoalDto);
    }

    public async Task<IEnumerable<SavingGoalDto>> GetUserSavingGoalsAsync(int userId)
    {
        var savingGoals = await _savingGoalRepository.GetByUserIdAsync(userId);
        return savingGoals.Select(MapToSavingGoalDto);
    }

    public async Task<IEnumerable<SavingGoalDto>> GetActiveSavingGoalsAsync(int userId)
    {
        var savingGoals = await _savingGoalRepository.GetActiveByUserIdAsync(userId);
        return savingGoals.Select(MapToSavingGoalDto);
    }

    public async Task<SavingGoalDto?> GetSavingGoalByIdAsync(int id)
    {
        var savingGoal = await _savingGoalRepository.GetByIdAsync(id);
        return savingGoal != null ? MapToSavingGoalDto(savingGoal) : null;
    }

    public async Task<SavingGoalDto> CreateSavingGoalAsync(int userId, CreateSavingGoalDto createSavingGoalDto)
    {
        var savingGoal = new SavingGoal
        {
            UserId = userId,
            Name = createSavingGoalDto.Name,
            Description = createSavingGoalDto.Description,
            TargetAmount = createSavingGoalDto.TargetAmount,
            CurrentAmount = 0,
            TargetDate = createSavingGoalDto.TargetDate,
            IconName = createSavingGoalDto.IconName,
            ColorCode = createSavingGoalDto.ColorCode,
            Status = "active"
        };

        var createdSavingGoal = await _savingGoalRepository.AddAsync(savingGoal);
        return MapToSavingGoalDto(createdSavingGoal);
    }

    public async Task<SavingGoalDto> UpdateSavingGoalAsync(int id, UpdateSavingGoalDto updateSavingGoalDto)
    {
        var savingGoal = await _savingGoalRepository.GetByIdAsync(id);
        if (savingGoal == null)
        {
            throw new ArgumentException("Saving goal not found");
        }

        if (!string.IsNullOrEmpty(updateSavingGoalDto.Name))
            savingGoal.Name = updateSavingGoalDto.Name;
        
        if (!string.IsNullOrEmpty(updateSavingGoalDto.Description))
            savingGoal.Description = updateSavingGoalDto.Description;
        
        if (updateSavingGoalDto.TargetAmount.HasValue)
            savingGoal.TargetAmount = updateSavingGoalDto.TargetAmount.Value;
        
        if (updateSavingGoalDto.TargetDate.HasValue)
            savingGoal.TargetDate = updateSavingGoalDto.TargetDate.Value;
        
        if (!string.IsNullOrEmpty(updateSavingGoalDto.IconName))
            savingGoal.IconName = updateSavingGoalDto.IconName;
        
        if (!string.IsNullOrEmpty(updateSavingGoalDto.ColorCode))
            savingGoal.ColorCode = updateSavingGoalDto.ColorCode;
        
        if (!string.IsNullOrEmpty(updateSavingGoalDto.Status))
            savingGoal.Status = updateSavingGoalDto.Status;

        var updatedSavingGoal = await _savingGoalRepository.UpdateAsync(savingGoal);
        return MapToSavingGoalDto(updatedSavingGoal);
    }

    public async Task<SavingGoalDto> AddToSavingGoalAsync(int id, AddToSavingGoalDto addToSavingGoalDto)
    {
        var savingGoal = await _savingGoalRepository.GetByIdAsync(id);
        if (savingGoal == null)
        {
            throw new ArgumentException("Saving goal not found");
        }

        savingGoal.CurrentAmount += addToSavingGoalDto.Amount;
        
        // Check if goal is completed
        if (savingGoal.CurrentAmount >= savingGoal.TargetAmount && savingGoal.Status == "active")
        {
            savingGoal.Status = "completed";
        }

        var updatedSavingGoal = await _savingGoalRepository.UpdateAsync(savingGoal);
        return MapToSavingGoalDto(updatedSavingGoal);
    }

    public async Task DeleteSavingGoalAsync(int id)
    {
        var savingGoal = await _savingGoalRepository.GetByIdAsync(id);
        if (savingGoal == null)
        {
            throw new ArgumentException("Saving goal not found");
        }

        await _savingGoalRepository.DeleteAsync(id);
    }

    private SavingGoalDto MapToSavingGoalDto(SavingGoal savingGoal)
    {
        return new SavingGoalDto
        {
            Id = savingGoal.Id,
            UserId = savingGoal.UserId,
            Name = savingGoal.Name,
            Description = savingGoal.Description,
            TargetAmount = savingGoal.TargetAmount,
            CurrentAmount = savingGoal.CurrentAmount,
            RemainingAmount = savingGoal.RemainingAmount,
            PercentageCompleted = savingGoal.PercentageCompleted,
            TargetDate = savingGoal.TargetDate,
            IconName = savingGoal.IconName,
            ColorCode = savingGoal.ColorCode,
            Status = savingGoal.Status,
            DaysRemaining = savingGoal.DaysRemaining,
            CreatedAt = savingGoal.CreatedAt,
            UpdatedAt = savingGoal.UpdatedAt
        };
    }
}