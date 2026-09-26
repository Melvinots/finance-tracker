using FinanceTracker.Models;
using FinanceTracker.Shared.DTOs.UserSettings;

namespace FinanceTracker.Repositories
{
    public interface IUserSettingsRepository
    {
        Task<UserSettings> GetAsync(int userId);
        Task<UserSettings> GetByIdAsync(int id, int userId);
        Task<UserSettings> UpdateAsync(UserSettings settings);
        Task<string> ExportDataAsync(int userId);
        Task DeactivateAccountAsync(int userId);
    }
}
