using FinanceTracker.Models;
using FinanceTracker.Repositories;
using FinanceTracker.Services;
using FinanceTracker.Shared.DTOs.UserSettings;
using Moq;

namespace FinanceTracker.Tests.Services
{
    public class UserSettingsServiceTests
    {
        private const int UserId = 5;

        private readonly Mock<IUserSettingsRepository> _repo = new();
        private readonly UserSettingsService _sut;

        public UserSettingsServiceTests()
        {
            _sut = new UserSettingsService(_repo.Object);
        }

        private static UserSettings SettingsFor(int id, string currency, string appearance) => new()
        {
            Id = id,
            Currency = currency,
            Appearance = appearance,
            User = new User { FullName = "Melvin", Email = "melvin@example.com" }
        };

        // ── GetAsync ─────────────────────────────────────────────────

        [Fact]
        public async Task GetAsync_MapsSettingsAndUserFields()
        {
            _repo.Setup(r => r.GetAsync(UserId))
                 .ReturnsAsync(SettingsFor(1, "PHP", "Dark"));

            var result = await _sut.GetAsync(UserId);

            Assert.Equal(1, result.Id);
            Assert.Equal("Melvin", result.FullName);
            Assert.Equal("melvin@example.com", result.Email);
            Assert.Equal("PHP", result.Currency);
            Assert.Equal("Dark", result.Appearance);
        }

        // ── UpdateAsync ──────────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_SavesNewValues_AndReturnsReloadedSettings()
        {
            var existing = SettingsFor(1, "USD", "Light");
            var reloaded = SettingsFor(1, "PHP", "Dark");

            // The service reads twice: before saving, then again after saving
            _repo.SetupSequence(r => r.GetAsync(UserId))
                 .ReturnsAsync(existing)
                 .ReturnsAsync(reloaded);

            var dto = new SaveUserSettingsDto { Currency = "PHP", Appearance = "Dark" };

            var result = await _sut.UpdateAsync(dto, UserId);

            // What was saved
            _repo.Verify(r => r.UpdateAsync(It.Is<UserSettings>(s =>
                s == existing &&
                s.Currency == "PHP" &&
                s.Appearance == "Dark")), Times.Once);

            // What came back comes from the reloaded copy
            Assert.Equal("PHP", result.Currency);
            Assert.Equal("Dark", result.Appearance);
            Assert.Equal("Melvin", result.FullName);
        }
    }
}
