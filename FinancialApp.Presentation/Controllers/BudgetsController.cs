using Microsoft.AspNetCore.Mvc;
using FinancialApp.Application.DTOs;
using FinancialApp.Application.Interfaces;

namespace FinancialApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetService _budgetService;

    public BudgetsController(IBudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BudgetDto>>> GetAllBudgets()
    {
        var budgets = await _budgetService.GetAllBudgetsAsync();
        return Ok(budgets);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<BudgetDto>>> GetUserBudgets(int userId)
    {
        var budgets = await _budgetService.GetUserBudgetsAsync(userId);
        return Ok(budgets);
    }

    [HttpGet("user/{userId}/period")]
    public async Task<ActionResult<IEnumerable<BudgetDto>>> GetUserBudgetsByPeriod(
        int userId, 
        [FromQuery] int month = 0, 
        [FromQuery] int year = 0)
    {
        // Use current month/year if not provided
        if (month == 0) month = DateTime.UtcNow.Month;
        if (year == 0) year = DateTime.UtcNow.Year;

        var budgets = await _budgetService.GetUserBudgetsByPeriodAsync(userId, month, year);
        return Ok(budgets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BudgetDto>> GetBudget(int id)
    {
        var budget = await _budgetService.GetBudgetByIdAsync(id);
        if (budget == null)
        {
            return NotFound();
        }
        return Ok(budget);
    }

    [HttpGet("user/{userId}/summary")]
    public async Task<ActionResult<BudgetSummaryDto>> GetBudgetSummary(
        int userId, 
        [FromQuery] int month = 0, 
        [FromQuery] int year = 0)
    {
        // Use current month/year if not provided
        if (month == 0) month = DateTime.UtcNow.Month;
        if (year == 0) year = DateTime.UtcNow.Year;

        var summary = await _budgetService.GetBudgetSummaryAsync(userId, month, year);
        return Ok(summary);
    }

    [HttpPost("user/{userId}")]
    public async Task<ActionResult<BudgetDto>> CreateBudget(int userId, CreateBudgetDto createBudgetDto)
    {
        try
        {
            var budget = await _budgetService.CreateBudgetAsync(userId, createBudgetDto);
            return CreatedAtAction(nameof(GetBudget), new { id = budget.Id }, budget);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BudgetDto>> UpdateBudget(int id, UpdateBudgetDto updateBudgetDto)
    {
        try
        {
            var budget = await _budgetService.UpdateBudgetAsync(id, updateBudgetDto);
            return Ok(budget);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBudget(int id)
    {
        try
        {
            await _budgetService.DeleteBudgetAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("update-spent")]
    public async Task<IActionResult> UpdateSpentAmount([FromBody] UpdateSpentAmountDto updateSpentDto)
    {
        await _budgetService.UpdateSpentAmountAsync(updateSpentDto.UserId, updateSpentDto.CategoryName, updateSpentDto.Amount);
        return Ok();
    }
}

public class UpdateSpentAmountDto
{
    public int UserId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}