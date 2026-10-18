
namespace FinanceTracker.Repositories
{
    public interface IUserSettingsRepository
    {
        Task<UserSettings> GetAsync(int userId);
        Task<UserSettings> UpdateAsync(UserSettings settings);
    }
}
