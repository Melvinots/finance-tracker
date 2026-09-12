using FinanceTracker.Repositories.Dashboard;
using FinanceTracker.Shared.DTOs.Dashboard;

namespace FinanceTracker.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repo;

        public DashboardService(IDashboardRepository repo)
        {
            _repo = repo;
        }

        public async Task<DashboardDto> GetDashboardAsync(int userId, int month, int year)
        {
            return await _repo.GetDashboardAsync(userId, month, year);
        }
    }
}
