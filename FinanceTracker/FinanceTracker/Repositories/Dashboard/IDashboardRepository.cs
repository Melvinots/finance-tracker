using FinanceTracker.Shared.DTOs.Dashboard;

namespace FinanceTracker.Repositories.Dashboard
{
    public interface IDashboardRepository
    {
        Task<DashboardDto> GetDashboardAsync(int userId, int month, int year);
    }
}
