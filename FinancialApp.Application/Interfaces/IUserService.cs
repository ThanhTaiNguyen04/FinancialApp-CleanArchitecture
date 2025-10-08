using FinancialApp.Application.DTOs;

namespace FinancialApp.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<UserDto?> GetUserByPhoneAsync(string phone);
    Task<UserDto> CreateUserAsync(UserDto userDto);
    Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
    Task<bool> DeleteUserAsync(int id);
    Task<FinancialSummaryDto> GetFinancialSummaryAsync(int userId);
}