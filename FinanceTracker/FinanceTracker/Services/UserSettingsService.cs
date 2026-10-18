using FinanceTracker.Repositories;

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

        public async Task<UserSettingsDto> UpdateAsync(SaveUserSettingsDto dto, int userId)
        {
            var settings = await _repo.GetAsync(userId)
                ?? throw new KeyNotFoundException("User settings not found.");

            settings.Currency = dto.Currency;
            settings.Appearance = dto.Appearance;

            await _repo.UpdateAsync(settings);

            var result = await _repo.GetAsync(userId);
            return MapToDto(result);
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
