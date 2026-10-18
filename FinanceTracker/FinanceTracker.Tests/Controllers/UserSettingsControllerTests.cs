using FinanceTracker.Controllers;
using FinanceTracker.Services;
using FinanceTracker.Shared.DTOs.UserSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;
using System.Security.Claims;

namespace FinanceTracker.Tests.Controllers
{
    public class UserSettingsControllerTests
    {
        private const int UserId = 5;

        private readonly Mock<IUserSettingsService> _service = new();
        private readonly UserSettingsController _sut;

        public UserSettingsControllerTests()
        {
            _sut = new UserSettingsController(_service.Object);
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

        // ── GetUserSettings ──────────────────────────────────────────

        [Fact]
        public async Task GetUserSettings_ReturnsOkWithSettingsForCurrentUser()
        {
            var dto = new UserSettingsDto { Id = 1, Currency = "PHP" };
            _service.Setup(s => s.GetAsync(UserId)).ReturnsAsync(dto);

            var result = await _sut.GetUserSettings();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(dto, ok.Value);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(42)]
        public async Task GetUserSettings_UsesUserIdFromClaims(int userId)
        {
            SetUser(userId);
            _service.Setup(s => s.GetAsync(userId)).ReturnsAsync(new UserSettingsDto());

            await _sut.GetUserSettings();

            _service.Verify(s => s.GetAsync(userId), Times.Once);
        }

        // ── UpdateUserSettings ───────────────────────────────────────

        [Fact]
        public async Task UpdateUserSettings_ForwardsIdDtoAndUser_ReturnsOk()
        {
            var input = new SaveUserSettingsDto { Currency = "PHP", Appearance = "Dark" };
            var updated = new UserSettingsDto { Id = 3, Currency = "PHP" };
            _service.Setup(s => s.UpdateAsync(input, UserId)).ReturnsAsync(updated);

            var result = await _sut.UpdateUserSettings(input);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(updated, ok.Value);
        }

        // ── Security ─────────────────────────────────────────────────

        [Fact]
        public void Controller_RequiresAuthorization()
        {
            var attribute = typeof(UserSettingsController)
                .GetCustomAttribute<AuthorizeAttribute>(inherit: true);

            Assert.NotNull(attribute);
        }
    }
}