using System.Security.Claims;
using FinancialApp.Application.DTOs;

namespace FinancialApp.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(int userId, string email, string fullName);
    ClaimsPrincipal? ValidateToken(string token);
}

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
    Task<UserDto?> GetUserProfileAsync(int userId);
    Task<UserDto?> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateDto);
}

public class UpdateUserProfileDto
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
}