using Microsoft.EntityFrameworkCore;
using FinancialApp.Domain.Entities;
using FinancialApp.Domain.Interfaces;
using FinancialApp.Infrastructure.Data;

namespace FinancialApp.Infrastructure.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly ApplicationDbContext _context;

    public BudgetRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Budget>> GetAllAsync()
    {
        return await _context.Budgets
            .Include(b => b.User)
            .Include(b => b.Category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Budget>> GetByUserIdAsync(int userId)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.Year)
            .ThenByDescending(b => b.Month)
            .ToListAsync();
    }

    public async Task<IEnumerable<Budget>> GetByUserIdAndPeriodAsync(int userId, int month, int year)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Where(b => b.UserId == userId && b.Month == month && b.Year == year)
            .ToListAsync();
    }

    public async Task<Budget?> GetByIdAsync(int id)
    {
        return await _context.Budgets
            .Include(b => b.User)
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Budget?> GetByUserCategoryAndPeriodAsync(int userId, int categoryId, int month, int year)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.UserId == userId && 
                                     b.CategoryId == categoryId && 
                                     b.Month == month && 
                                     b.Year == year);
    }

    public async Task<Budget> AddAsync(Budget budget)
    {
        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();
        return budget;
    }

    public async Task<Budget> UpdateAsync(Budget budget)
    {
        budget.UpdatedAt = DateTime.UtcNow;
        _context.Entry(budget).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return budget;
    }

    public async Task DeleteAsync(int id)
    {
        var budget = await _context.Budgets.FindAsync(id);
        if (budget != null)
        {
            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Budgets.AnyAsync(b => b.Id == id);
    }
}