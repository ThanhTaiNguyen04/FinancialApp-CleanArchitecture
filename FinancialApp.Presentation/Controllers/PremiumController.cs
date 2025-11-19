using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinancialApp.Infrastructure.Data;
using FinancialApp.Application.DTOs;
using FinancialApp.Domain.Entities;

namespace FinancialApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PremiumController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<PremiumController> _logger;

    public PremiumController(ApplicationDbContext context, ILogger<PremiumController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // POST: api/Premium/request
    [HttpPost("request")]
    public async Task<ActionResult<PremiumRequestDto>> CreatePremiumRequest([FromBody] CreatePremiumRequestDto dto)
    {
        try
        {
            var userId = int.Parse(Request.Headers["X-User-Id"].ToString());

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Người dùng không tồn tại" });
            }

            var hasPendingRequest = await _context.PremiumRequests
                .AnyAsync(pr => pr.UserId == userId && pr.Status == "Pending");

            if (hasPendingRequest)
            {
                return BadRequest(new { message = "Bạn đã có yêu cầu đang chờ xử lý" });
            }

            // Auto-generate transaction reference if not provided
            string transactionRef = dto.TransactionReference;
            if (string.IsNullOrEmpty(transactionRef))
            {
                transactionRef = $"QR_AUTO_PREMIUM_{DateTime.UtcNow:yyyyMMddHHmmss}_{userId}";
            }

            var premiumRequest = new PremiumRequest
            {
                UserId = userId,
                Amount = 29000,
                TransactionReference = transactionRef,
                Status = "Pending",
                RequestDate = DateTime.UtcNow
            };

            _context.PremiumRequests.Add(premiumRequest);
            await _context.SaveChangesAsync();

            await _context.Entry(premiumRequest).Reference(pr => pr.User).LoadAsync();

            var resultDto = new PremiumRequestDto
            {
                Id = premiumRequest.Id,
                UserId = premiumRequest.UserId,
                UserName = premiumRequest.User?.FullName ?? "",
                UserEmail = premiumRequest.User?.Email ?? "",
                Amount = premiumRequest.Amount,
                TransactionReference = premiumRequest.TransactionReference,
                Status = premiumRequest.Status,
                RequestDate = premiumRequest.RequestDate
            };

            return CreatedAtAction(nameof(GetPremiumRequest), new { id = premiumRequest.Id }, resultDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating premium request");
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    // GET: api/Premium/pending
    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<PremiumRequestDto>>> GetPendingRequests()
    {
        try
        {
            var pendingRequests = await _context.PremiumRequests
                .Include(pr => pr.User)
                .Where(pr => pr.Status == "Pending")
                .OrderByDescending(pr => pr.RequestDate)
                .Select(pr => new PremiumRequestDto
                {
                    Id = pr.Id,
                    UserId = pr.UserId,
                    UserName = pr.User != null ? pr.User.FullName : "",
                    UserEmail = pr.User != null ? pr.User.Email : "",
                    Amount = pr.Amount,
                    TransactionReference = pr.TransactionReference,
                    Status = pr.Status,
                    RequestDate = pr.RequestDate,
                    ApprovedDate = pr.ApprovedDate,
                    ApprovedBy = pr.ApprovedBy,
                    RejectionReason = pr.RejectionReason
                })
                .ToListAsync();

            return Ok(pendingRequests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching pending requests");
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    // GET: api/Premium/all
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<PremiumRequestDto>>> GetAllRequests()
    {
        try
        {
            var allRequests = await _context.PremiumRequests
                .Include(pr => pr.User)
                .Include(pr => pr.ApprovedByUser)
                .OrderByDescending(pr => pr.RequestDate)
                .Select(pr => new PremiumRequestDto
                {
                    Id = pr.Id,
                    UserId = pr.UserId,
                    UserName = pr.User != null ? pr.User.FullName : "",
                    UserEmail = pr.User != null ? pr.User.Email : "",
                    Amount = pr.Amount,
                    TransactionReference = pr.TransactionReference,
                    Status = pr.Status,
                    RequestDate = pr.RequestDate,
                    ApprovedDate = pr.ApprovedDate,
                    ApprovedBy = pr.ApprovedBy,
                    ApprovedByName = pr.ApprovedByUser != null ? pr.ApprovedByUser.FullName : null,
                    RejectionReason = pr.RejectionReason
                })
                .ToListAsync();

            return Ok(allRequests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all requests");
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    // GET: api/Premium/request/{id}
    [HttpGet("request/{id}")]
    public async Task<ActionResult<PremiumRequestDto>> GetPremiumRequest(int id)
    {
        try
        {
            var request = await _context.PremiumRequests
                .Include(pr => pr.User)
                .Include(pr => pr.ApprovedByUser)
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (request == null)
            {
                return NotFound(new { message = "Không tìm thấy yêu cầu" });
            }

            var dto = new PremiumRequestDto
            {
                Id = request.Id,
                UserId = request.UserId,
                UserName = request.User?.FullName ?? "",
                UserEmail = request.User?.Email ?? "",
                Amount = request.Amount,
                TransactionReference = request.TransactionReference,
                Status = request.Status,
                RequestDate = request.RequestDate,
                ApprovedDate = request.ApprovedDate,
                ApprovedBy = request.ApprovedBy,
                ApprovedByName = request.ApprovedByUser?.FullName,
                RejectionReason = request.RejectionReason
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching request {Id}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    // PUT: api/Premium/approve/{id}
    [HttpPut("approve/{id}")]
    public async Task<ActionResult<PremiumRequestDto>> ApprovePremiumRequest(int id, [FromBody] ApprovePremiumRequestDto dto)
    {
        try
        {
            var adminId = int.Parse(Request.Headers["X-User-Id"].ToString());

            var admin = await _context.Users.FindAsync(adminId);
            
            // Log for debugging
            _logger.LogInformation($"Approve request - Admin ID: {adminId}, Admin Role: '{admin?.Role}', Email: '{admin?.Email}'");
            
            if (admin == null)
            {
                return Unauthorized(new { message = "Không tìm thấy admin user" });
            }
            
            // Check if user is admin (case-insensitive, support both "admin" and "Admin")
            bool isAdmin = !string.IsNullOrEmpty(admin.Role) && 
                          admin.Role.Equals("admin", StringComparison.OrdinalIgnoreCase);
            
            if (!isAdmin)
            {
                _logger.LogWarning($"User {adminId} ({admin.Email}) attempted to approve but role is '{admin.Role}'");
                return Unauthorized(new { message = $"Không có quyền. Role hiện tại: '{admin.Role}'" });
            }

            var request = await _context.PremiumRequests
                .Include(pr => pr.User)
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (request == null)
            {
                return NotFound(new { message = "Không tìm thấy yêu cầu" });
            }

            if (request.Status != "Pending")
            {
                return BadRequest(new { message = "Yêu cầu đã được xử lý" });
            }

            request.Status = dto.Approved ? "Approved" : "Rejected";
            request.ApprovedDate = DateTime.UtcNow;
            request.ApprovedBy = adminId;
            request.RejectionReason = dto.Approved ? null : "Từ chối bởi admin";

            if (dto.Approved && request.User != null)
            {
                request.User.IsPremium = true;
                request.User.SubscriptionType = "Premium";
                
                if (request.User.PremiumExpiry.HasValue && request.User.PremiumExpiry > DateTime.UtcNow)
                {
                    request.User.PremiumExpiry = request.User.PremiumExpiry.Value.AddMonths(1);
                }
                else
                {
                    request.User.PremiumExpiry = DateTime.UtcNow.AddMonths(1);
                }
            }

            await _context.SaveChangesAsync();

            await _context.Entry(request).Reference(pr => pr.ApprovedByUser).LoadAsync();

            var resultDto = new PremiumRequestDto
            {
                Id = request.Id,
                UserId = request.UserId,
                UserName = request.User?.FullName ?? "",
                UserEmail = request.User?.Email ?? "",
                Amount = request.Amount,
                TransactionReference = request.TransactionReference,
                Status = request.Status,
                RequestDate = request.RequestDate,
                ApprovedDate = request.ApprovedDate,
                ApprovedBy = request.ApprovedBy,
                ApprovedByName = request.ApprovedByUser?.FullName,
                RejectionReason = request.RejectionReason
            };

            return Ok(resultDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving request {Id}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    // PUT: api/Premium/revoke/{id}
    // Revoke an approved premium request and remove user's premium status
    [HttpPut("revoke/{id}")]
    public async Task<ActionResult> RevokePremiumRequest(int id, [FromBody] RevokePremiumRequestDto revokeDto)
    {
        try
        {
            var request = await _context.PremiumRequests
                .Include(pr => pr.User)
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (request == null)
            {
                return NotFound(new { message = "Không tìm thấy yêu cầu" });
            }

            if (request.Status != "Approved")
            {
                return BadRequest(new { message = "Chỉ có thể hủy các yêu cầu đã được duyệt" });
            }

            // Update request status to Revoked
            request.Status = "Revoked";
            request.RejectionReason = revokeDto.Reason ?? "Premium bị hủy bởi admin";
            request.ApprovedDate = null; // Clear approval date
            request.ApprovedBy = null;   // Clear approver

            // Remove user's premium status
            if (request.User != null)
            {
                request.User.IsPremium = false;
                request.User.PremiumExpiry = null;
                request.User.SubscriptionType = "Free";
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Premium request {id} has been revoked. User {request.UserId} is no longer Premium.");

            return Ok(new
            {
                message = "Đã hủy duyệt thành công",
                requestId = id,
                userId = request.UserId,
                status = request.Status,
                revokedAmount = request.Amount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking request {Id}", id);
            return StatusCode(500, new { message = "Đã xảy ra lỗi khi hủy duyệt" });
        }
    }

    // GET: api/Premium/user/{userId}/requests
    [HttpGet("user/{userId}/requests")]
    public async Task<ActionResult<IEnumerable<PremiumRequestDto>>> GetUserRequests(int userId)
    {
        try
        {
            var userRequests = await _context.PremiumRequests
                .Include(pr => pr.ApprovedByUser)
                .Where(pr => pr.UserId == userId)
                .OrderByDescending(pr => pr.RequestDate)
                .Select(pr => new PremiumRequestDto
                {
                    Id = pr.Id,
                    UserId = pr.UserId,
                    Amount = pr.Amount,
                    TransactionReference = pr.TransactionReference,
                    Status = pr.Status,
                    RequestDate = pr.RequestDate,
                    ApprovedDate = pr.ApprovedDate,
                    ApprovedBy = pr.ApprovedBy,
                    ApprovedByName = pr.ApprovedByUser != null ? pr.ApprovedByUser.FullName : null,
                    RejectionReason = pr.RejectionReason
                })
                .ToListAsync();

            return Ok(userRequests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user requests {UserId}", userId);
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    // GET: api/Premium/check/{userId}
    [HttpGet("check/{userId}")]
    public async Task<ActionResult<object>> CheckPremiumStatus(int userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Người dùng không tồn tại" });
            }

            if (user.IsPremium && user.PremiumExpiry.HasValue && user.PremiumExpiry < DateTime.UtcNow)
            {
                user.IsPremium = false;
                user.SubscriptionType = "Free";
                await _context.SaveChangesAsync();
            }

            return Ok(new
            {
                isPremium = user.IsPremium,
                subscriptionType = user.SubscriptionType,
                premiumExpiry = user.PremiumExpiry,
                daysRemaining = user.PremiumExpiry.HasValue
                    ? Math.Max(0, (user.PremiumExpiry.Value - DateTime.UtcNow).Days)
                    : 0
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking status {UserId}", userId);
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    // GET: api/Premium/stats
    [HttpGet("stats")]
    public async Task<ActionResult<object>> GetPremiumStats()
    {
        try
        {
            var totalRequests = await _context.PremiumRequests.CountAsync();
            var pendingRequests = await _context.PremiumRequests.CountAsync(pr => pr.Status == "Pending");
            var approvedRequests = await _context.PremiumRequests.CountAsync(pr => pr.Status == "Approved");
            var rejectedRequests = await _context.PremiumRequests.CountAsync(pr => pr.Status == "Rejected");
            var revokedRequests = await _context.PremiumRequests.CountAsync(pr => pr.Status == "Revoked");
            
            // Calculate revenue correctly
            // Total revenue = All approved requests (current revenue)
            var totalRevenue = await _context.PremiumRequests
                .Where(pr => pr.Status == "Approved")
                .SumAsync(pr => pr.Amount);
                
            // Refund amount = All revoked requests (money to refund, not deducted from revenue)
            var totalRefunds = await _context.PremiumRequests
                .Where(pr => pr.Status == "Revoked")
                .SumAsync(pr => pr.Amount);
                
            // Net revenue = Current revenue (for display purposes)
            // Note: Refunds are handled separately and don't reduce historical revenue
            
            var activePremiumUsers = await _context.Users
                .CountAsync(u => u.IsPremium && u.PremiumExpiry > DateTime.UtcNow);

            var firstDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var requestsThisMonth = await _context.PremiumRequests
                .CountAsync(pr => pr.RequestDate >= firstDayOfMonth);
            var revenueThisMonth = await _context.PremiumRequests
                .Where(pr => pr.Status == "Approved" && pr.ApprovedDate >= firstDayOfMonth)
                .SumAsync(pr => pr.Amount);
            var refundsThisMonth = await _context.PremiumRequests
                .Where(pr => pr.Status == "Revoked" && pr.RequestDate >= firstDayOfMonth)
                .SumAsync(pr => pr.Amount);

            return Ok(new
            {
                totalRequests,
                pendingRequests,
                approvedRequests,
                rejectedRequests,
                revokedRequests,
                totalRevenue,
                totalRefunds,
                activePremiumUsers,
                requestsThisMonth,
                revenueThisMonth,
                refundsThisMonth
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching stats");
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    // DELETE: api/Premium/cancel/{userId} - FOR TESTING ONLY
    [HttpDelete("cancel/{userId}")]
    public async Task<ActionResult> CancelPremium(int userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Người dùng không tồn tại" });
            }

            // Reset premium status
            user.IsPremium = false;
            user.SubscriptionType = "Free";
            user.PremiumExpiry = null;

            // Delete all premium requests for testing
            var requests = await _context.PremiumRequests
                .Where(pr => pr.UserId == userId)
                .ToListAsync();
            _context.PremiumRequests.RemoveRange(requests);

            await _context.SaveChangesAsync();

            return Ok(new { 
                message = "Đã hủy Premium thành công",
                userId,
                isPremium = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error canceling premium {UserId}", userId);
            return StatusCode(500, new { message = "Đã xảy ra lỗi" });
        }
    }

    // GET: api/Premium/status
    // Kiểm tra trạng thái Premium của user hiện tại
    [HttpGet("status")]
    public async Task<ActionResult> GetPremiumStatus()
    {
        try
        {
            var userIdHeader = Request.Headers["X-User-Id"].FirstOrDefault();
            int userId;
            if (string.IsNullOrEmpty(userIdHeader) || !int.TryParse(userIdHeader, out userId))
            {
                // For testing, use User ID 3 if no header provided
                userId = 3;
                _logger.LogWarning($"No X-User-Id header provided, using fallback User ID: {userId}");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "Người dùng không tồn tại" });
            }

            // Kiểm tra trạng thái Premium
            var isPremium = user.IsPremium && user.PremiumExpiry > DateTime.UtcNow;
            
            // Debug logging
            _logger.LogInformation($"Premium Status Debug - UserId: {userId}, IsPremium: {user.IsPremium}, PremiumExpiry: {user.PremiumExpiry}, UtcNow: {DateTime.UtcNow}, Result: {isPremium}");
            
            return Ok(new
            {
                isPremium = isPremium,
                premiumExpiry = user.PremiumExpiry,
                message = isPremium ? "User đang có Premium" : "User chưa có Premium"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting premium status for user");
            return StatusCode(500, new { message = "Lỗi khi kiểm tra trạng thái Premium" });
        }
    }
}
