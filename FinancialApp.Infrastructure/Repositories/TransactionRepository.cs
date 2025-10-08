using Microsoft.EntityFrameworkCore;
using FinancialApp.Domain.Entities;
using FinancialApp.Domain.Interfaces;
using FinancialApp.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace FinancialApp.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TransactionRepository> _logger;

    public TransactionRepository(ApplicationDbContext context, ILogger<TransactionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync()
    {
        _logger.LogWarning("GetAllAsync called - This method returns ALL transactions from ALL users and should not be used in production");
        var transactions = await _context.Transactions.Include(t => t.User).ToListAsync();
        _logger.LogInformation("Retrieved {Count} transactions from database (ALL users)", transactions.Count);
        return transactions;
    }

    public async Task<IEnumerable<Transaction>> GetByUserIdAsync(int userId)
    {
        _logger.LogInformation("GetByUserIdAsync called for userId: {UserId}", userId);
        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
        _logger.LogInformation("Retrieved {Count} transactions for userId: {UserId} from database", transactions.Count, userId);
        return transactions;
    }

    public async Task<Transaction?> GetByIdAsync(int id)
    {
        _logger.LogInformation("GetByIdAsync called for transactionId: {TransactionId}", id);
        var transaction = await _context.Transactions
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id);
        
        if (transaction != null)
        {
            _logger.LogInformation("Found transaction {TransactionId} belonging to userId: {UserId}", id, transaction.UserId);
        }
        else
        {
            _logger.LogWarning("Transaction {TransactionId} not found in database", id);
        }
        
        return transaction;
    }

    public async Task<Transaction> AddAsync(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<Transaction> UpdateAsync(Transaction transaction)
    {
        _context.Entry(transaction).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task DeleteAsync(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);
        if (transaction != null)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId && t.TransactionDate >= startDate && t.TransactionDate <= endDate)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Transaction>> GetByTypeAsync(int userId, string type)
    {
        return await _context.Transactions
            .Where(t => t.UserId == userId && t.Type == type)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
    }
}