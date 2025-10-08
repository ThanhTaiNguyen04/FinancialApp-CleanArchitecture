using FinancialApp.Application.DTOs;
using FinancialApp.Application.Interfaces;
using FinancialApp.Domain.Entities;
using FinancialApp.Domain.Interfaces;

namespace FinancialApp.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ITransactionRepository _transactionRepository;

    public UserService(IUserRepository userRepository, ITransactionRepository transactionRepository)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto?> GetUserByPhoneAsync(string phone)
    {
        var user = await _userRepository.GetByPhoneAsync(phone);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto> CreateUserAsync(UserDto userDto)
    {
        var user = MapToEntity(userDto);
        var createdUser = await _userRepository.AddAsync(user);
        return MapToDto(createdUser);
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null) return null;

        if (!string.IsNullOrEmpty(updateUserDto.FullName))
            existingUser.FullName = updateUserDto.FullName;
        
        if (!string.IsNullOrEmpty(updateUserDto.Phone))
            existingUser.Phone = updateUserDto.Phone;
        
        if (!string.IsNullOrEmpty(updateUserDto.AvatarUrl))
            existingUser.AvatarUrl = updateUserDto.AvatarUrl;

        var updatedUser = await _userRepository.UpdateAsync(existingUser);
        return MapToDto(updatedUser);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var exists = await _userRepository.ExistsAsync(id);
        if (!exists) return false;

        await _userRepository.DeleteAsync(id);
        return true;
    }

    public async Task<FinancialSummaryDto> GetFinancialSummaryAsync(int userId)
    {
        var transactions = await _transactionRepository.GetByUserIdAsync(userId);
        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;

        var totalIncome = transactions.Where(t => t.Type == "income").Sum(t => t.Amount);
        var totalExpenses = transactions.Where(t => t.Type == "expense").Sum(t => t.Amount);
        var monthlySpent = transactions
            .Where(t => t.Type == "expense" && t.TransactionDate.Month == currentMonth && t.TransactionDate.Year == currentYear)
            .Sum(t => t.Amount);

        return new FinancialSummaryDto
        {
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            MonthlySpent = monthlySpent
        };
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            AvatarUrl = user.AvatarUrl,
            AvailableBalance = user.AvailableBalance,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    private static User MapToEntity(UserDto userDto)
    {
        return new User
        {
            FullName = userDto.FullName,
            Email = userDto.Email,
            Phone = userDto.Phone,
            AvatarUrl = userDto.AvatarUrl,
            AvailableBalance = userDto.AvailableBalance
        };
    }
}