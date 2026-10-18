using FinanceTracker.Repositories;
using FinanceTracker.Services;
using Moq;

namespace FinanceTracker.Tests.Services
{
    public class AccountServiceTests
    {
        private const int UserId = 5;

        private readonly Mock<IAccountRepository> _repo = new();
        private readonly AccountService _sut;

        public AccountServiceTests()
        {
            _sut = new AccountService(_repo.Object);
        }

        // ── ExportDataAsync ──────────────────────────────────────────

        [Fact]
        public async Task ExportDataAsync_ReturnsCsvFromRepository_ForCurrentUser()
        {
            _repo.Setup(r => r.ExportDataAsync(UserId)).ReturnsAsync("Date,Amount\n2026-10-01,50");

            var result = await _sut.ExportDataAsync(UserId);

            Assert.Equal("Date,Amount\n2026-10-01,50", result);
        }

        // ── DeactivateAccountAsync ───────────────────────────────────

        [Fact]
        public async Task DeactivateAccountAsync_DeactivatesOnlyTheGivenUser()
        {
            await _sut.DeactivateAccountAsync(UserId);

            _repo.Verify(r => r.DeactivateAccountAsync(UserId), Times.Once);
            _repo.Verify(r => r.DeactivateAccountAsync(It.Is<int>(id => id != UserId)), Times.Never);
        }
    }
}
