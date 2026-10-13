using FinanceTracker.Controllers;
using FinanceTracker.Services.Dashboard;
using FinanceTracker.Shared.DTOs.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;
using System.Security.Claims;

namespace FinanceTracker.Tests.Controllers
{
    public class DashboardControllerTests
    {
        private const int UserId = 5;

        private readonly Mock<IDashboardService> _service = new();
        private readonly DashboardController _sut;

        public DashboardControllerTests()
        {
            _sut = new DashboardController(_service.Object);
            SetUser(UserId);
        }

        private void SetUser(int userId)
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            _sut.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
            };
        }

        [Fact]
        public async Task GetDashboard_ReturnsOkWithDashboard()
        {
            var dashboard = new DashboardDto();
            _service.Setup(s => s.GetDashboardAsync(UserId, 10, 2026)).ReturnsAsync(dashboard);

            var result = await _sut.GetDashboard(10, 2026);

            // ActionResult<T>: the response is in .Result
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(dashboard, ok.Value);
        }

        [Fact]
        public async Task GetDashboard_ForwardsMonthAndYearInOrder()
        {
            _service.Setup(s => s.GetDashboardAsync(UserId, 10, 2026)).ReturnsAsync(new DashboardDto());

            await _sut.GetDashboard(month: 10, year: 2026);

            _service.Verify(s => s.GetDashboardAsync(UserId, 10, 2026), Times.Once);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(42)]
        public async Task GetDashboard_UsesUserIdFromClaims(int userId)
        {
            SetUser(userId);
            _service.Setup(s => s.GetDashboardAsync(userId, 10, 2026)).ReturnsAsync(new DashboardDto());

            await _sut.GetDashboard(10, 2026);

            _service.Verify(s => s.GetDashboardAsync(userId, 10, 2026), Times.Once);
        }

        [Fact]
        public void Controller_RequiresAuthorization()
        {
            var attribute = typeof(DashboardController)
                .GetCustomAttribute<AuthorizeAttribute>(inherit: true);

            Assert.NotNull(attribute);
        }
    }
}