using Microsoft.AspNetCore.Mvc;
using FinancialApp.Application.DTOs;
using FinancialApp.Application.Interfaces;

namespace FinancialApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SavingGoalsController : ControllerBase
{
    private readonly ISavingGoalService _savingGoalService;

    public SavingGoalsController(ISavingGoalService savingGoalService)
    {
        _savingGoalService = savingGoalService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SavingGoalDto>>> GetAllSavingGoals()
    {
        var savingGoals = await _savingGoalService.GetAllSavingGoalsAsync();
        return Ok(savingGoals);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<SavingGoalDto>>> GetUserSavingGoals(int userId)
    {
        var savingGoals = await _savingGoalService.GetUserSavingGoalsAsync(userId);
        return Ok(savingGoals);
    }

    [HttpGet("user/{userId}/active")]
    public async Task<ActionResult<IEnumerable<SavingGoalDto>>> GetActiveSavingGoals(int userId)
    {
        var savingGoals = await _savingGoalService.GetActiveSavingGoalsAsync(userId);
        return Ok(savingGoals);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SavingGoalDto>> GetSavingGoal(int id)
    {
        var savingGoal = await _savingGoalService.GetSavingGoalByIdAsync(id);
        if (savingGoal == null)
        {
            return NotFound();
        }
        return Ok(savingGoal);
    }

    [HttpPost("user/{userId}")]
    public async Task<ActionResult<SavingGoalDto>> CreateSavingGoal(int userId, CreateSavingGoalDto createSavingGoalDto)
    {
        var savingGoal = await _savingGoalService.CreateSavingGoalAsync(userId, createSavingGoalDto);
        return CreatedAtAction(nameof(GetSavingGoal), new { id = savingGoal.Id }, savingGoal);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SavingGoalDto>> UpdateSavingGoal(int id, UpdateSavingGoalDto updateSavingGoalDto)
    {
        try
        {
            var savingGoal = await _savingGoalService.UpdateSavingGoalAsync(id, updateSavingGoalDto);
            return Ok(savingGoal);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{id}/add-money")]
    public async Task<ActionResult<SavingGoalDto>> AddToSavingGoal(int id, AddToSavingGoalDto addToSavingGoalDto)
    {
        try
        {
            var savingGoal = await _savingGoalService.AddToSavingGoalAsync(id, addToSavingGoalDto);
            return Ok(savingGoal);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSavingGoal(int id)
    {
        try
        {
            await _savingGoalService.DeleteSavingGoalAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}