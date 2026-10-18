using FinanceTracker.Controllers;
using FinanceTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Text;

namespace FinanceTracker.Tests.Controllers
{
    public class AccountControllerTests
    {
        private const int UserId = 5;

        private readonly Mock<IAccountService> _service = new();
        private readonly AccountController _sut;

        public AccountControllerTests()
        {
            _sut = new AccountController(_service.Object);
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
        public async Task DeactivateAccount_ReturnsNoContentAndDeactivatesCurrentUser()
        {
            var result = await _sut.DeactivateAccount();

            Assert.IsType<NoContentResult>(result);
            _service.Verify(s => s.DeactivateAccountAsync(UserId), Times.Once);
        }

        // ── Security ─────────────────────────────────────────────────

        [Fact]
        public void Controller_RequiresAuthorization()
        {
            var attribute = typeof(AccountController)
                .GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true);

            Assert.NotNull(attribute);
        }
    }
}
