using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinancialApp.Application.DTOs;
using FinancialApp.Application.Interfaces;
using System.Security.Claims;

namespace FinancialApp.Presentation.Controllers;

[Route("api/[controller]")]
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Đăng ký tài khoản mới
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var result = await _authService.RegisterAsync(registerDto);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Đăng ký thành công", 201));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(ex.Message, 400));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("Đã xảy ra lỗi trong quá trình đăng ký", 500, ex.Message));
        }
    }

    /// <summary>
    /// Đăng nhập
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginAsync(loginDto);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Đăng nhập thành công"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse(ex.Message, 401));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("Đã xảy ra lỗi trong quá trình đăng nhập", 500, ex.Message));
        }
    }

    /// <summary>
    /// Lấy thông tin profile người dùng hiện tại
    /// </summary>
    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _authService.GetUserProfileAsync(userId);
            
            if (result == null)
                return NotFound(ApiResponse<UserDto>.ErrorResponse("Không tìm thấy thông tin người dùng", 404));
                
            return Ok(ApiResponse<UserDto>.SuccessResponse(result, "Lấy thông tin profile thành công"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("Đã xảy ra lỗi khi lấy thông tin profile", 500, ex.Message));
        }
    }

    /// <summary>
    /// Cập nhật thông tin profile
    /// </summary>
    [HttpPut("profile")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateProfile([FromBody] UpdateUserProfileDto updateDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _authService.UpdateUserProfileAsync(userId, updateDto);
            
            if (result == null)
                return NotFound(ApiResponse<UserDto>.ErrorResponse("Không tìm thấy thông tin người dùng", 404));
                
            return Ok(ApiResponse<UserDto>.SuccessResponse(result, "Cập nhật profile thành công"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("Đã xảy ra lỗi khi cập nhật profile", 500, ex.Message));
        }
    }

    /// <summary>
    /// Đổi mật khẩu
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var success = await _authService.ChangePasswordAsync(userId, changePasswordDto);
            
            if (!success)
                return NotFound(ApiResponse.ErrorResponse("Không tìm thấy thông tin người dùng", 404));
                
            return Ok(ApiResponse.SuccessResponse("Đổi mật khẩu thành công"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse.ErrorResponse(ex.Message, 401));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.ErrorResponse("Đã xảy ra lỗi khi đổi mật khẩu", 500, ex.Message));
        }
    }

    /// <summary>
    /// Test endpoint để kiểm tra authentication
    /// </summary>
    [HttpGet("test")]
    [Authorize]
    public IActionResult Test()
    {
        var userId = GetCurrentUserId();
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        
        var testData = new { 
            userId = userId,
            email = email,
            name = name,
            timestamp = DateTime.UtcNow
        };
        
        return Ok(ApiResponse<object>.SuccessResponse(testData, "Authentication thành công!"));
    }
}