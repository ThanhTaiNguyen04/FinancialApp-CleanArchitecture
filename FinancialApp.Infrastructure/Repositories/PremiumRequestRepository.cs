using Microsoft.EntityFrameworkCore;
using FinancialApp.Domain.Entities;
using FinancialApp.Domain.Interfaces;
using FinancialApp.Infrastructure.Data;

namespace FinancialApp.Infrastructure.Repositories;

public class PremiumRequestRepository : IPremiumRequestRepository
{
    private readonly ApplicationDbContext _context;

    public PremiumRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PremiumRequest>> GetAllAsync()
    {
        return await _context.PremiumRequests
            .Include(p => p.User)
            .Include(p => p.ApprovedByUser)
            .ToListAsync();
    }

    public async Task<PremiumRequest?> GetByIdAsync(int id)
    {
        return await _context.PremiumRequests
            .Include(p => p.User)
            .Include(p => p.ApprovedByUser)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<PremiumRequest>> GetByUserIdAsync(int userId)
    {
        return await _context.PremiumRequests
            .Where(p => p.UserId == userId)
            .Include(p => p.User)
            .Include(p => p.ApprovedByUser)
            .ToListAsync();
    }

    public async Task<PremiumRequest> AddAsync(PremiumRequest premiumRequest)
    {
        _context.PremiumRequests.Add(premiumRequest);
        await _context.SaveChangesAsync();
        return premiumRequest;
    }

    public async Task<PremiumRequest> UpdateAsync(PremiumRequest premiumRequest)
    {
        _context.Entry(premiumRequest).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return premiumRequest;
    }

    public async Task DeleteAsync(int id)
    {
        var premiumRequest = await _context.PremiumRequests.FindAsync(id);
        if (premiumRequest != null)
        {
            _context.PremiumRequests.Remove(premiumRequest);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.PremiumRequests.AnyAsync(p => p.Id == id);
    }
}
