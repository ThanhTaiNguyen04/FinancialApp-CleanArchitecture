using FinancialApp.Application.DTOs;

namespace FinancialApp.Application.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync();
    Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(int userId);
    Task<TransactionDto?> GetTransactionByIdAsync(int id);
    Task<TransactionDto> CreateTransactionAsync(int userId, CreateTransactionDto createTransactionDto);
    Task<TransactionDto> UpdateTransactionAsync(int id, CreateTransactionDto updateTransactionDto);
    Task DeleteTransactionAsync(int id);
    Task<IEnumerable<CategoryStatsDto>> GetCategoryStatsAsync(int userId, string period = "monthly");
}