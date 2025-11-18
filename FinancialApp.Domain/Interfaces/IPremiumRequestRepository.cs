using FinancialApp.Domain.Entities;

namespace FinancialApp.Domain.Interfaces
{
    public interface IPremiumRequestRepository
    {
        Task<IEnumerable<PremiumRequest>> GetAllAsync();
        Task<PremiumRequest?> GetByIdAsync(int id);
        Task<IEnumerable<PremiumRequest>> GetByUserIdAsync(int userId);
        Task<PremiumRequest> AddAsync(PremiumRequest premiumRequest);
        Task<PremiumRequest> UpdateAsync(PremiumRequest premiumRequest);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
