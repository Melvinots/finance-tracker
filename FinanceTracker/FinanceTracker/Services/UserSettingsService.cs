using FinanceTracker.Models;
using FinanceTracker.Repositories;
using FinanceTracker.Shared.DTOs.UserSettings;

namespace FinanceTracker.Services
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly IUserSettingsRepository _repo;

        public UserSettingsService(IUserSettingsRepository repo)
        {
            _repo = repo;
        }

        public async Task<UserSettingsDto> GetAsync(int userId)
        {
            var userSettings = await _repo.GetAsync(userId);
            return MapToDto(userSettings);
        }

        public async Task<UserSettingsDto> UpdateAsync(int id, SaveUserSettingsDto dto, int userId)
        {
            var settings = await _repo.GetByIdAsync(id, userId);

            settings.Currency = dto.Currency;
            settings.Appearance = dto.Appearance;

            await _repo.UpdateAsync(settings);

            var result = await _repo.GetByIdAsync(id, userId);
            return MapToDto(result);
        }

        public async Task<string> ExportDataAsync(int userId)
        {
            return await _repo.ExportDataAsync(userId);
        }

        public async Task DeactivateAccountAsync(int userId)
        {
            throw new NotImplementedException();
        }

        // ── private helpers ──────────────────────────────────────────

        private static UserSettingsDto MapToDto(UserSettings us) => new()
        {
            Id = us.Id,
            FullName = us.User.FullName,
            Email = us.User.Email,
            Currency = us.Currency,
            Appearance= us.Appearance
        };
    }
}
