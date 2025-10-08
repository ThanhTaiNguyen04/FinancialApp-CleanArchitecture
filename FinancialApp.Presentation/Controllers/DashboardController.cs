using Microsoft.AspNetCore.Mvc;
using FinancialApp.Application.DTOs;
using FinancialApp.Application.Interfaces;

namespace FinancialApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<DashboardDto>> GetDashboardData(int userId)
    {
        var dashboardData = await _dashboardService.GetDashboardDataAsync(userId);
        return Ok(dashboardData);
    }

    [HttpGet("user/{userId}/account-balance")]
    public async Task<ActionResult<AccountBalanceDto>> GetAccountBalance(
        int userId, 
        [FromQuery] string period = "monthly")
    {
        var accountBalance = await _dashboardService.GetAccountBalanceAsync(userId, period);
        return Ok(accountBalance);
    }

    [HttpGet("user/{userId}/chart")]
    public async Task<ActionResult<FinancialChartDto>> GetFinancialChart(
        int userId, 
        [FromQuery] string period = "daily", 
        [FromQuery] int days = 30)
    {
        var chartData = await _dashboardService.GetFinancialChartDataAsync(userId, period, days);
        return Ok(chartData);
    }
}