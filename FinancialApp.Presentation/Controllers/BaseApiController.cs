using Microsoft.AspNetCore.Mvc;
using FinancialApp.Application.DTOs;
using System.Security.Claims;

namespace FinancialApp.Presentation.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    protected string GetCurrentUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    }

    protected string GetCurrentUserName()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
    }

    protected ActionResult<ApiResponse<T>> SuccessResponse<T>(T data, string message = "Thành công", int statusCode = 200)
    {
        return StatusCode(statusCode, ApiResponse<T>.SuccessResponse(data, message, statusCode));
    }

    protected ActionResult<ApiResponse> SuccessResponse(string message = "Thành công", int statusCode = 200)
    {
        return StatusCode(statusCode, ApiResponse.SuccessResponse(message, statusCode));
    }

    protected ActionResult<ApiResponse<T>> ErrorResponse<T>(string message, int statusCode = 400, object? errors = null)
    {
        return StatusCode(statusCode, ApiResponse<T>.ErrorResponse(message, statusCode, errors));
    }

    protected ActionResult<ApiResponse> ErrorResponse(string message, int statusCode = 400, object? errors = null)
    {
        return StatusCode(statusCode, ApiResponse.ErrorResponse(message, statusCode, errors));
    }

    protected ActionResult<PaginatedResponse<T>> PaginatedResponse<T>(
        List<T> data, 
        int page, 
        int pageSize, 
        int totalItems,
        string message = "Thành công")
    {
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        
        return Ok(new PaginatedResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Pagination = new PaginationInfo
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasNextPage = page < totalPages,
                HasPreviousPage = page > 1
            }
        });
    }
}