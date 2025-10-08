using BCrypt.Net;
using FinancialApp.Application.DTOs;
using FinancialApp.Application.Interfaces;
using FinancialApp.Domain.Entities;
using FinancialApp.Domain.Interfaces;

namespace FinancialApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // Kiểm tra email đã tồn tại
        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email đã được sử dụng.");
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        // Tạo user mới
        var user = new User
        {
            FullName = registerDto.FullName,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            Phone = registerDto.Phone,
            AvatarUrl = "https://images.pexels.com/photos/2167673/pexels-photo-2167673.jpeg?auto=compress&cs=tinysrgb&dpr=3&h=750&w=1260",
            AvailableBalance = 0m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        // Generate token
        var token = _jwtService.GenerateToken(user.Id, user.Email, user.FullName);

        return new AuthResponseDto
        {
            Token = token,
            Expires = DateTime.UtcNow.AddHours(24),
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                AvatarUrl = user.AvatarUrl,
                AvailableBalance = user.AvailableBalance,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");
        }

        // Generate token
        var token = _jwtService.GenerateToken(user.Id, user.Email, user.FullName);

        return new AuthResponseDto
        {
            Token = token,
            Expires = DateTime.UtcNow.AddHours(24),
            User = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                AvatarUrl = user.AvatarUrl,
                AvailableBalance = user.AvailableBalance,
                CreatedAt = user.CreatedAt
            }
        };
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return false;

        if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Mật khẩu hiện tại không đúng.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<UserDto?> GetUserProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            AvatarUrl = user.AvatarUrl,
            AvailableBalance = user.AvailableBalance,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<UserDto?> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        user.FullName = updateDto.FullName;
        user.Phone = updateDto.Phone;
        user.AvatarUrl = updateDto.AvatarUrl;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            AvatarUrl = user.AvatarUrl,
            AvailableBalance = user.AvailableBalance,
            CreatedAt = user.CreatedAt
        };
    }
}