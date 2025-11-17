using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FinancialApp.Infrastructure.Data;
using FinancialApp.Application.DTOs;
using FinancialApp.Domain.Entities;
using System.Text.Json;

namespace FinancialApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(ApplicationDbContext context, ILogger<WebhookController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Webhook để nhận thông báo thanh toán từ ngân hàng/cổng thanh toán
    /// </summary>
    [HttpPost("payment-notification")]
    public async Task<IActionResult> PaymentNotification([FromBody] PaymentWebhookDto webhookData)
    {
        try
        {
            _logger.LogInformation("Received payment webhook: {Data}", JsonSerializer.Serialize(webhookData));

            // Validate webhook signature (implement based on payment provider)
            if (!ValidateWebhookSignature(webhookData))
            {
                _logger.LogWarning("Invalid webhook signature");
                return BadRequest(new { message = "Invalid signature" });
            }

            // Find premium request by transaction reference
            var premiumRequest = await _context.PremiumRequests
                .Include(pr => pr.User)
                .FirstOrDefaultAsync(pr => 
                    pr.TransactionReference == webhookData.TransactionReference &&
                    pr.Status == "Pending");

            if (premiumRequest == null)
            {
                _logger.LogWarning("Premium request not found for transaction: {TransactionRef}", webhookData.TransactionReference);
                return NotFound(new { message = "Premium request not found" });
            }

            // Validate payment amount
            if (webhookData.Amount != premiumRequest.Amount)
            {
                _logger.LogWarning("Payment amount mismatch. Expected: {Expected}, Received: {Received}", 
                    premiumRequest.Amount, webhookData.Amount);
                return BadRequest(new { message = "Amount mismatch" });
            }

            // Check if payment is successful
            if (webhookData.Status.ToLower() == "success")
            {
                await ActivatePremiumAccount(premiumRequest, webhookData);
                return Ok(new { message = "Premium activated successfully" });
            }
            else
            {
                await RejectPremiumRequest(premiumRequest, $"Payment failed: {webhookData.Status}");
                return Ok(new { message = "Premium request rejected due to payment failure" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment webhook");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Manual API để admin kích hoạt Premium (dùng cho QR Code payment)
    /// </summary>
    [HttpPost("manual-activation")]
    public async Task<IActionResult> ManualActivation([FromBody] ManualActivationDto activationData)
    {
        try
        {
            // Validate admin authentication
            var adminIdHeader = Request.Headers["X-User-Id"].FirstOrDefault();
            if (string.IsNullOrEmpty(adminIdHeader) || !int.TryParse(adminIdHeader, out int adminId))
            {
                return BadRequest(new { message = "Admin ID is required" });
            }

            var admin = await _context.Users.FindAsync(adminId);
            if (admin == null || (admin.Role != "admin" && admin.Role != "Admin"))
            {
                return Forbid("Only admins can manually activate premium");
            }

            // Find premium request
            var premiumRequest = await _context.PremiumRequests
                .Include(pr => pr.User)
                .FirstOrDefaultAsync(pr => pr.Id == activationData.RequestId);

            if (premiumRequest == null)
            {
                return NotFound(new { message = "Premium request not found" });
            }

            if (premiumRequest.Status != "Pending")
            {
                return BadRequest(new { message = "Premium request is not pending" });
            }

            // Create mock payment webhook data for manual activation
            var mockWebhookData = new PaymentWebhookDto
            {
                TransactionReference = premiumRequest.TransactionReference ?? $"MANUAL_{DateTime.UtcNow:yyyyMMddHHmmss}",
                Amount = premiumRequest.Amount,
                Status = "Success",
                PaymentMethod = "QR_Code",
                PaymentDate = DateTime.UtcNow,
                BankTransactionId = activationData.BankTransactionId
            };

            await ActivatePremiumAccount(premiumRequest, mockWebhookData);

            return Ok(new { 
                message = "Premium activated manually", 
                userId = premiumRequest.UserId,
                premiumExpiry = premiumRequest.User.PremiumExpiry
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in manual premium activation");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// API để kiểm tra trạng thái thanh toán
    /// </summary>
    [HttpGet("payment-status/{transactionReference}")]
    public async Task<IActionResult> CheckPaymentStatus(string transactionReference)
    {
        try
        {
            var premiumRequest = await _context.PremiumRequests
                .Include(pr => pr.User)
                .FirstOrDefaultAsync(pr => pr.TransactionReference == transactionReference);

            if (premiumRequest == null)
            {
                return NotFound(new { message = "Transaction not found" });
            }

            return Ok(new
            {
                transactionReference,
                status = premiumRequest.Status,
                amount = premiumRequest.Amount,
                requestDate = premiumRequest.RequestDate,
                approvedDate = premiumRequest.ApprovedDate,
                user = new
                {
                    id = premiumRequest.UserId,
                    name = premiumRequest.User.FullName,
                    email = premiumRequest.User.Email,
                    isPremium = premiumRequest.User.IsPremium,
                    premiumExpiry = premiumRequest.User.PremiumExpiry
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking payment status");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    private async Task ActivatePremiumAccount(PremiumRequest premiumRequest, PaymentWebhookDto webhookData)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Update premium request status
            premiumRequest.Status = "Approved";
            premiumRequest.ApprovedDate = DateTime.UtcNow;
            premiumRequest.ApprovedBy = 1; // System auto-approval

            // Update user premium status
            var user = premiumRequest.User;
            user.IsPremium = true;
            user.SubscriptionType = "Premium";
            
            // Set premium expiry (30 days from now)
            user.PremiumExpiry = DateTime.UtcNow.AddDays(30);
            
            // Update timestamp
            user.UpdatedAt = DateTime.UtcNow;

            // Save changes
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Premium activated for user {UserId} via transaction {TransactionRef}", 
                user.Id, webhookData.TransactionReference);

            // Here you could send notification email to user
            // await SendPremiumActivationEmail(user);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error activating premium for user {UserId}", premiumRequest.UserId);
            throw;
        }
    }

    private async Task RejectPremiumRequest(PremiumRequest premiumRequest, string reason)
    {
        premiumRequest.Status = "Rejected";
        premiumRequest.RejectionReason = reason;
        premiumRequest.ApprovedDate = DateTime.UtcNow;
        premiumRequest.ApprovedBy = 1; // System auto-rejection

        await _context.SaveChangesAsync();

        _logger.LogInformation("Premium request rejected for user {UserId}: {Reason}", 
            premiumRequest.UserId, reason);
    }

    private bool ValidateWebhookSignature(PaymentWebhookDto webhookData)
    {
        // Implement webhook signature validation based on your payment provider
        // For example, verify HMAC signature from headers
        
        // For now, return true (implement proper validation in production)
        return true;
    }
}

// DTOs for webhook
public class PaymentWebhookDto
{
    public string TransactionReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty; // Success, Failed, Pending
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string BankTransactionId { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
}

public class ManualActivationDto
{
    public int RequestId { get; set; }
    public string BankTransactionId { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}