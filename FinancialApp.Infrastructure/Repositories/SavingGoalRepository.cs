using Microsoft.EntityFrameworkCore;
using FinancialApp.Domain.Entities;
using FinancialApp.Domain.Interfaces;
using FinancialApp.Infrastructure.Data;

namespace FinancialApp.Infrastructure.Repositories;

public class SavingGoalRepository : ISavingGoalRepository
{
    private readonly ApplicationDbContext _context;

    public SavingGoalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SavingGoal>> GetAllAsync()
    {
        return await _context.SavingGoals
            .Include(s => s.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<SavingGoal>> GetByUserIdAsync(int userId)
    {
        return await _context.SavingGoals
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<SavingGoal>> GetActiveByUserIdAsync(int userId)
    {
        return await _context.SavingGoals
            .Where(s => s.UserId == userId && s.Status == "active")
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<SavingGoal?> GetByIdAsync(int id)
    {
        return await _context.SavingGoals
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<SavingGoal> AddAsync(SavingGoal savingGoal)
    {
        _context.SavingGoals.Add(savingGoal);
        await _context.SaveChangesAsync();
        return savingGoal;
    }

    public async Task<SavingGoal> UpdateAsync(SavingGoal savingGoal)
    {
        savingGoal.UpdatedAt = DateTime.UtcNow;
        _context.Entry(savingGoal).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return savingGoal;
    }

    public async Task DeleteAsync(int id)
    {
        var savingGoal = await _context.SavingGoals.FindAsync(id);
        if (savingGoal != null)
        {
            _context.SavingGoals.Remove(savingGoal);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.SavingGoals.AnyAsync(s => s.Id == id);
    }
}