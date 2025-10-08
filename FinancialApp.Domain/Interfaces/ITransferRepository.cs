using FinancialApp.Domain.Entities;

namespace FinancialApp.Domain.Interfaces;

public interface ITransferRepository
{
    Task<IEnumerable<Transfer>> GetAllAsync();
    Task<IEnumerable<Transfer>> GetByUserIdAsync(int userId);
    Task<Transfer?> GetByIdAsync(int id);
    Task<Transfer> AddAsync(Transfer transfer);
    Task<Transfer> UpdateAsync(Transfer transfer);
    Task DeleteAsync(int id);
    Task<IEnumerable<Transfer>> GetByStatusAsync(string status);
    Task<IEnumerable<Transfer>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
}