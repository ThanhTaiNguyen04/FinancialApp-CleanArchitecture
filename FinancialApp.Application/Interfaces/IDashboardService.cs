using FinancialApp.Application.DTOs;

namespace FinancialApp.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardDataAsync(int userId);
    Task<AccountBalanceDto> GetAccountBalanceAsync(int userId, string period = "monthly");
    Task<FinancialChartDto> GetFinancialChartDataAsync(int userId, string period = "daily", int days = 30);
    Task<AdminStatsDto> GetAdminStatsAsync();
}