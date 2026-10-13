using FinanceTracker.Controllers;
using FinanceTracker.Services;
using FinanceTracker.Shared.DTOs.Settings;
using FinanceTracker.Shared.DTOs.UserSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

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

            var ok = Assert.IsType<OkObjectResult>(result);
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
            _service.Setup(s => s.UpdateAsync(3, input, UserId)).ReturnsAsync(updated);

            var result = await _sut.UpdateUserSettings(3, input);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(updated, ok.Value);
        }

        // ── Export ───────────────────────────────────────────────────

        [Fact]
        public async Task ExportData_ReturnsCsvFileWithContent()
        {
            const string csv = "Date,Amount\n2026-10-01,50";
            _service.Setup(s => s.ExportDataAsync(UserId)).ReturnsAsync(csv);

            var result = await _sut.ExportData();

            var file = Assert.IsType<FileContentResult>(result);
            Assert.Equal("text/csv", file.ContentType);
            Assert.Equal(csv, Encoding.UTF8.GetString(file.FileContents));
        }

        [Fact]
        public async Task ExportData_FileNameHasExpectedFormat()
        {
            _service.Setup(s => s.ExportDataAsync(UserId)).ReturnsAsync("x");

            var result = await _sut.ExportData();

            var file = Assert.IsType<FileContentResult>(result);
            // Regex instead of an exact date, so the test can't fail around midnight UTC
            Assert.Matches(@"^financetracker_export_\d{8}\.csv$", file.FileDownloadName);
        }

        [Fact]
        public async Task ExportData_ExportsOnlyCurrentUsersData()
        {
            SetUser(42);
            _service.Setup(s => s.ExportDataAsync(42)).ReturnsAsync("x");

            await _sut.ExportData();

            _service.Verify(s => s.ExportDataAsync(42), Times.Once);
            _service.Verify(s => s.ExportDataAsync(UserId), Times.Never);
        }

        // ── Deactivate ───────────────────────────────────────────────

        [Theory]
        [InlineData(5)]
        [InlineData(42)]
        public async Task DeactivateAccount_DeactivatesTheLoggedInUser(int userId)
        {
            SetUser(userId);

            await _sut.DeactivateAccount();

            _service.Verify(s => s.DeactivateAccountAsync(userId), Times.Once);
        }

        [Fact]
        public async Task DeactivateAccount_ReturnsOkWithMessage()
        {
            var result = await _sut.DeactivateAccount();

            var ok = Assert.IsType<OkObjectResult>(result);
            // The response is an anonymous type, so read "Message" via reflection
            var message = ok.Value!.GetType().GetProperty("Message")!.GetValue(ok.Value);
            Assert.Equal("Account deactivated successfully.", message);
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