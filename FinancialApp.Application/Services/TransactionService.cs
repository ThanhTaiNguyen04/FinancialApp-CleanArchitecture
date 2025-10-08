using FinancialApp.Application.DTOs;
using FinancialApp.Application.Interfaces;
using FinancialApp.Domain.Entities;
using FinancialApp.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FinancialApp.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBudgetService _budgetService;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(ITransactionRepository transactionRepository, IBudgetService budgetService, ILogger<TransactionService> logger)
    {
        _transactionRepository = transactionRepository;
        _budgetService = budgetService;
        _logger = logger;
    }

    public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
    {
        _logger.LogInformation("GetAllTransactionsAsync called - WARNING: This should not be used in production as it returns all users' data");
        var transactions = await _transactionRepository.GetAllAsync();
        _logger.LogInformation("Retrieved {Count} transactions from ALL users", transactions.Count());
        return transactions.Select(MapToTransactionDto);
    }

    public async Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(int userId)
    {
        _logger.LogInformation("GetUserTransactionsAsync called for userId: {UserId}", userId);
        var transactions = await _transactionRepository.GetByUserIdAsync(userId);
        _logger.LogInformation("Retrieved {Count} transactions for userId: {UserId}", transactions.Count(), userId);
        return transactions.Select(MapToTransactionDto);
    }

    public async Task<TransactionDto?> GetTransactionByIdAsync(int id)
    {
        _logger.LogInformation("GetTransactionByIdAsync called for transactionId: {TransactionId}", id);
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction != null)
        {
            _logger.LogInformation("Found transaction {TransactionId} belonging to userId: {UserId}", id, transaction.UserId);
        }
        else
        {
            _logger.LogWarning("Transaction {TransactionId} not found", id);
        }
        return transaction != null ? MapToTransactionDto(transaction) : null;
    }

    public async Task<TransactionDto> CreateTransactionAsync(int userId, CreateTransactionDto createTransactionDto)
    {
        var transaction = new Transaction
        {
            UserId = userId,
            Type = createTransactionDto.Type,
            Category = createTransactionDto.Category,
            Amount = createTransactionDto.Amount,
            Description = createTransactionDto.Description,
            IconName = createTransactionDto.IconName,
            TransactionDate = createTransactionDto.TransactionDate ?? DateTime.UtcNow
        };

        var createdTransaction = await _transactionRepository.AddAsync(transaction);

        // Update budget spent amount if it's an expense
        if (transaction.Type == "expense")
        {
            await _budgetService.UpdateSpentAmountAsync(userId, transaction.Category, transaction.Amount);
        }

        return MapToTransactionDto(createdTransaction);
    }

    public async Task<TransactionDto> UpdateTransactionAsync(int id, CreateTransactionDto updateTransactionDto)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null)
        {
            throw new ArgumentException("Transaction not found");
        }

        var oldCategory = transaction.Category;
        var oldAmount = transaction.Amount;
        var oldType = transaction.Type;

        transaction.Type = updateTransactionDto.Type;
        transaction.Category = updateTransactionDto.Category;
        transaction.Amount = updateTransactionDto.Amount;
        transaction.Description = updateTransactionDto.Description;
        transaction.IconName = updateTransactionDto.IconName;
        if (updateTransactionDto.TransactionDate.HasValue)
            transaction.TransactionDate = updateTransactionDto.TransactionDate.Value;

        var updatedTransaction = await _transactionRepository.UpdateAsync(transaction);

        // Update budget spent amounts
        if (oldType == "expense")
        {
            await _budgetService.UpdateSpentAmountAsync(transaction.UserId, oldCategory, -oldAmount);
        }
        if (transaction.Type == "expense")
        {
            await _budgetService.UpdateSpentAmountAsync(transaction.UserId, transaction.Category, transaction.Amount);
        }

        return MapToTransactionDto(updatedTransaction);
    }

    public async Task DeleteTransactionAsync(int id)
    {
        var transaction = await _transactionRepository.GetByIdAsync(id);
        if (transaction == null)
        {
            throw new ArgumentException("Transaction not found");
        }

        // Update budget spent amount if it was an expense
        if (transaction.Type == "expense")
        {
            await _budgetService.UpdateSpentAmountAsync(transaction.UserId, transaction.Category, -transaction.Amount);
        }

        await _transactionRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<CategoryStatsDto>> GetCategoryStatsAsync(int userId, string period = "monthly")
    {
        var transactions = await _transactionRepository.GetByUserIdAsync(userId);
        
        var filteredTransactions = FilterTransactionsByPeriod(transactions, period);
        var expenses = filteredTransactions.Where(t => t.Type == "expense").ToList();
        var totalExpenses = expenses.Sum(t => t.Amount);

        return expenses
            .GroupBy(t => t.Category)
            .Select(g => new CategoryStatsDto
            {
                Category = g.Key,
                Amount = g.Sum(t => t.Amount),
                Percentage = totalExpenses > 0 ? (g.Sum(t => t.Amount) / totalExpenses) * 100 : 0
            })
            .OrderByDescending(c => c.Amount)
            .ToList();
    }

    private IEnumerable<Transaction> FilterTransactionsByPeriod(IEnumerable<Transaction> transactions, string period)
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

    private TransactionDto MapToTransactionDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            UserId = transaction.UserId,
            Type = transaction.Type,
            Category = transaction.Category,
            Amount = transaction.Amount,
            Description = transaction.Description,
            IconName = transaction.IconName,
            TransactionDate = transaction.TransactionDate,
            CreatedAt = transaction.CreatedAt
        };
    }
}