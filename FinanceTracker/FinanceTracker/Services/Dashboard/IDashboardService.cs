using FinanceTracker.Shared.DTOs.Dashboard;

namespace FinanceTracker.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync(int userId, int month, int year);
    }
}
