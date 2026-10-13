using FinanceTracker.Repositories.Dashboard;
using FinanceTracker.Services.Dashboard;
using FinanceTracker.Shared.DTOs.Dashboard;
using Moq;

namespace FinanceTracker.Tests.Services
{
    public class DashboardServiceTests
    {
        private const int UserId = 5;

        private readonly Mock<IDashboardRepository> _repo = new();
        private readonly DashboardService _sut;

        public DashboardServiceTests()
        {
            _sut = new DashboardService(_repo.Object);
        }

        [Fact]
        public async Task GetDashboardAsync_ReturnsDashboardFromRepository()
        {
            var dashboard = new DashboardDto();
            _repo.Setup(r => r.GetDashboardAsync(UserId, 10, 2026)).ReturnsAsync(dashboard);

            var result = await _sut.GetDashboardAsync(UserId, 10, 2026);

            Assert.Same(dashboard, result);
        }

        // Distinct values (10 vs 2026) so a swapped month/year would not match the Setup
        [Fact]
        public async Task GetDashboardAsync_ForwardsUserMonthAndYearInOrder()
        {
            _repo.Setup(r => r.GetDashboardAsync(UserId, 10, 2026)).ReturnsAsync(new DashboardDto());

            await _sut.GetDashboardAsync(UserId, 10, 2026);

            _repo.Verify(r => r.GetDashboardAsync(UserId, 10, 2026), Times.Once);
        }

        [Fact]
        public async Task GetDashboardAsync_WhenRepositoryThrows_PropagatesException()
        {
            _repo.Setup(r => r.GetDashboardAsync(UserId, 10, 2026))
                 .ThrowsAsync(new InvalidOperationException("db down"));

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.GetDashboardAsync(UserId, 10, 2026));
        }
    }
}