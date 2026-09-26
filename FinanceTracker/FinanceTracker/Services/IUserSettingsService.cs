using FinanceTracker.Shared.DTOs.UserSettings;

namespace FinanceTracker.Services
{
    public interface IUserSettingsService
    {
        Task<UserSettingsDto> GetAsync(int userId);
        Task<UserSettingsDto> UpdateAsync(int id, SaveUserSettingsDto dto, int userId);
        Task<string> ExportDataAsync(int userId);
        Task DeactivateAccountAsync(int userId);
    }
}
