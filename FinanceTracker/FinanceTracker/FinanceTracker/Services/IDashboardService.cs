using FinanceTracker.Shared.DTOs.Dashboard;

namespace FinanceTracker.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync(int userId, int month, int year);
    }
}
