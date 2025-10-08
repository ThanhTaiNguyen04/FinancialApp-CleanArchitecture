using Microsoft.AspNetCore.Mvc;
using FinancialApp.Application.DTOs;
using FinancialApp.Application.Interfaces;

namespace FinancialApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IBudgetService _budgetService;
    private readonly ISavingGoalService _savingGoalService;

    public SyncController(
        ITransactionService transactionService,
        IBudgetService budgetService,
        ISavingGoalService savingGoalService)
    {
        _transactionService = transactionService;
        _budgetService = budgetService;
        _savingGoalService = savingGoalService;
    }

    [HttpGet("user/{userId}/full-data")]
    public async Task<ActionResult<FullUserDataDto>> GetFullUserData(int userId)
    {
        var transactions = await _transactionService.GetUserTransactionsAsync(userId);
        var budgets = await _budgetService.GetUserBudgetsAsync(userId);
        var savingGoals = await _savingGoalService.GetUserSavingGoalsAsync(userId);

        return Ok(new FullUserDataDto
        {
            Transactions = transactions.ToList(),
            Budgets = budgets.ToList(),
            SavingGoals = savingGoals.ToList(),
            LastSyncTime = DateTime.UtcNow
        });
    }

    [HttpPost("user/{userId}/sync")]
    public async Task<ActionResult<SyncResultDto>> SyncUserData(int userId, SyncDataDto syncData)
    {
        var result = new SyncResultDto();

        try
        {
            // Sync Transactions
            foreach (var transaction in syncData.Transactions)
            {
                if (transaction.Id == 0) // New transaction
                {
                    await _transactionService.CreateTransactionAsync(userId, new CreateTransactionDto
                    {
                        Type = transaction.Type,
                        Category = transaction.Category,
                        Amount = transaction.Amount,
                        Description = transaction.Description,
                        TransactionDate = transaction.TransactionDate
                    });
                    result.TransactionsSynced++;
                }
            }

            // Sync Budgets
            foreach (var budget in syncData.Budgets)
            {
                if (budget.Id == 0) // New budget
                {
                    await _budgetService.CreateBudgetAsync(userId, new CreateBudgetDto
                    {
                        CategoryId = budget.CategoryId,
                        BudgetAmount = budget.BudgetAmount,
                        Month = budget.Month,
                        Year = budget.Year
                    });
                    result.BudgetsSynced++;
                }
            }

            // Sync Saving Goals
            foreach (var goal in syncData.SavingGoals)
            {
                if (goal.Id == 0) // New saving goal
                {
                    await _savingGoalService.CreateSavingGoalAsync(userId, new CreateSavingGoalDto
                    {
                        Name = goal.Name,
                        Description = goal.Description,
                        TargetAmount = goal.TargetAmount,
                        TargetDate = goal.TargetDate,
                        IconName = goal.IconName,
                        ColorCode = goal.ColorCode
                    });
                    result.SavingGoalsSynced++;
                }
            }

            result.Success = true;
            result.LastSyncTime = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }

        return Ok(result);
    }

    [HttpGet("user/{userId}/last-sync")]
    public async Task<ActionResult<DateTime>> GetLastSyncTime(int userId)
    {
        // In a real app, you would store this in database
        // For now, just return current time
        return Ok(DateTime.UtcNow);
    }
}

public class FullUserDataDto
{
    public List<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
    public List<BudgetDto> Budgets { get; set; } = new List<BudgetDto>();
    public List<SavingGoalDto> SavingGoals { get; set; } = new List<SavingGoalDto>();
    public DateTime LastSyncTime { get; set; }
}

public class SyncDataDto
{
    public List<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
    public List<BudgetDto> Budgets { get; set; } = new List<BudgetDto>();
    public List<SavingGoalDto> SavingGoals { get; set; } = new List<SavingGoalDto>();
    public DateTime LastSyncTime { get; set; }
}

public class SyncResultDto
{
    public bool Success { get; set; }
    public int TransactionsSynced { get; set; }
    public int BudgetsSynced { get; set; }
    public int SavingGoalsSynced { get; set; }
    public DateTime LastSyncTime { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}