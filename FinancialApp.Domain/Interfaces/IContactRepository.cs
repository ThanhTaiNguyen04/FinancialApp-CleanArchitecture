using FinancialApp.Domain.Entities;

namespace FinancialApp.Domain.Interfaces;

public interface IContactRepository
{
    Task<IEnumerable<Contact>> GetAllAsync();
    Task<IEnumerable<Contact>> GetByUserIdAsync(int userId);
    Task<Contact?> GetByIdAsync(int id);
    Task<Contact> AddAsync(Contact contact);
    Task<Contact> UpdateAsync(Contact contact);
    Task DeleteAsync(int id);
    Task<IEnumerable<Contact>> GetRecentContactsAsync(int userId);
    Task<Contact?> GetByPhoneAsync(string phone);
}