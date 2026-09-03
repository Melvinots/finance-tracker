using FinanceTracker.Shared.DTOs.Settings;

namespace FinanceTracker.Services
{
    public interface ISettingsService
    {
        Task<SettingsDto> GetAsync();
        Task UpdateAsync(SettingsDto settings);
        Task<string> ExportDataAsync();
        Task DeleteAccountAsync();
    }
}
