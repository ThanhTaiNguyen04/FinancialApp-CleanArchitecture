using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinancialApp.Application.DTOs;
using FinancialApp.Application.Interfaces;
using System.Security.Claims;

namespace FinancialApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetAllTransactions()
    {
        var currentUserId = GetCurrentUserId();
        _logger.LogInformation("GetAllTransactions called for userId: {UserId}", currentUserId);
        
        var transactions = await _transactionService.GetUserTransactionsAsync(currentUserId);
        _logger.LogInformation("Retrieved {Count} transactions for userId: {UserId}", transactions.Count(), currentUserId);
        
        return Ok(transactions);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<TransactionDto>>> GetUserTransactions(int userId)
    {
        var currentUserId = GetCurrentUserId();
        _logger.LogInformation("GetUserTransactions called with userId: {RequestedUserId}, current user: {CurrentUserId}", userId, currentUserId);
        
        // Security check: Users can only access their own transactions
        if (userId != currentUserId)
        {
            _logger.LogWarning("Unauthorized access attempt: User {CurrentUserId} tried to access transactions of user {RequestedUserId}", currentUserId, userId);
            return Forbid("You can only access your own transactions");
        }
        
        var transactions = await _transactionService.GetUserTransactionsAsync(userId);
        _logger.LogInformation("Retrieved {Count} transactions for userId: {UserId}", transactions.Count(), userId);
        
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
    {
        var currentUserId = GetCurrentUserId();
        _logger.LogInformation("GetTransaction called for transactionId: {TransactionId}, current user: {UserId}", id, currentUserId);
        
        var transaction = await _transactionService.GetTransactionByIdAsync(id);
        if (transaction == null)
        {
            _logger.LogWarning("Transaction {TransactionId} not found", id);
            return NotFound();
        }
        
        // Security check: Users can only access their own transactions
        if (transaction.UserId != currentUserId)
        {
            _logger.LogWarning("Unauthorized access attempt: User {CurrentUserId} tried to access transaction {TransactionId} belonging to user {OwnerUserId}", currentUserId, id, transaction.UserId);
            return Forbid("You can only access your own transactions");
        }
        
        return Ok(transaction);
    }

    [HttpPost("user/{userId}")]
    public async Task<ActionResult<TransactionDto>> CreateTransaction(int userId, CreateTransactionDto createTransactionDto)
    {
        var currentUserId = GetCurrentUserId();
        _logger.LogInformation("CreateTransaction called for userId: {RequestedUserId}, current user: {CurrentUserId}", userId, currentUserId);
        
        // Security check: Users can only create transactions for themselves
        if (userId != currentUserId)
        {
            _logger.LogWarning("Unauthorized creation attempt: User {CurrentUserId} tried to create transaction for user {RequestedUserId}", currentUserId, userId);
            return Forbid("You can only create transactions for yourself");
        }
        
        var transaction = await _transactionService.CreateTransactionAsync(userId, createTransactionDto);
        _logger.LogInformation("Created transaction {TransactionId} for user {UserId}", transaction.Id, userId);
        
        return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TransactionDto>> UpdateTransaction(int id, CreateTransactionDto updateTransactionDto)
    {
        var currentUserId = GetCurrentUserId();
        _logger.LogInformation("UpdateTransaction called for transactionId: {TransactionId}, current user: {UserId}", id, currentUserId);
        
        try
        {
            // First check if transaction exists and belongs to current user
            var existingTransaction = await _transactionService.GetTransactionByIdAsync(id);
            if (existingTransaction == null)
            {
                _logger.LogWarning("Transaction {TransactionId} not found for update", id);
                return NotFound("Transaction not found");
            }
            
            if (existingTransaction.UserId != currentUserId)
            {
                _logger.LogWarning("Unauthorized update attempt: User {CurrentUserId} tried to update transaction {TransactionId} belonging to user {OwnerUserId}", currentUserId, id, existingTransaction.UserId);
                return Forbid("You can only update your own transactions");
            }
            
            var transaction = await _transactionService.UpdateTransactionAsync(id, updateTransactionDto);
            _logger.LogInformation("Updated transaction {TransactionId} for user {UserId}", id, currentUserId);
            
            return Ok(transaction);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("Error updating transaction {TransactionId}: {Error}", id, ex.Message);
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(int id)
    {
        var currentUserId = GetCurrentUserId();
        _logger.LogInformation("DeleteTransaction called for transactionId: {TransactionId}, current user: {UserId}", id, currentUserId);
        
        try
        {
            // First check if transaction exists and belongs to current user
            var existingTransaction = await _transactionService.GetTransactionByIdAsync(id);
            if (existingTransaction == null)
            {
                _logger.LogWarning("Transaction {TransactionId} not found for deletion", id);
                return NotFound("Transaction not found");
            }
            
            if (existingTransaction.UserId != currentUserId)
            {
                _logger.LogWarning("Unauthorized delete attempt: User {CurrentUserId} tried to delete transaction {TransactionId} belonging to user {OwnerUserId}", currentUserId, id, existingTransaction.UserId);
                return Forbid("You can only delete your own transactions");
            }
            
            await _transactionService.DeleteTransactionAsync(id);
            _logger.LogInformation("Deleted transaction {TransactionId} for user {UserId}", id, currentUserId);
            
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            _logger.LogError("Error deleting transaction {TransactionId}: {Error}", id, ex.Message);
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<TransactionDto>> CreateTransactionForCurrentUser(CreateTransactionDto createTransactionDto)
    {
        var currentUserId = GetCurrentUserId();
        _logger.LogInformation("CreateTransactionForCurrentUser called for user: {UserId}", currentUserId);
        
        var transaction = await _transactionService.CreateTransactionAsync(currentUserId, createTransactionDto);
        _logger.LogInformation("Created transaction {TransactionId} for user {UserId}", transaction.Id, currentUserId);
        
        return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
    }

    [HttpGet("user/{userId}/category-stats")]
    public async Task<ActionResult<IEnumerable<CategoryStatsDto>>> GetCategoryStats(
        int userId, 
        [FromQuery] string period = "monthly")
    {
        var currentUserId = GetCurrentUserId();
        _logger.LogInformation("GetCategoryStats called for userId: {RequestedUserId}, current user: {CurrentUserId}", userId, currentUserId);
        
        // Security check: Users can only access their own stats
        if (userId != currentUserId)
        {
            _logger.LogWarning("Unauthorized access attempt: User {CurrentUserId} tried to access category stats of user {RequestedUserId}", currentUserId, userId);
            return Forbid("You can only access your own category stats");
        }
        
        var stats = await _transactionService.GetCategoryStatsAsync(userId, period);
        return Ok(stats);
    }
    
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            _logger.LogError("Unable to extract user ID from JWT token");
            throw new UnauthorizedAccessException("Invalid or missing user ID in token");
        }
        return userId;
    }
}