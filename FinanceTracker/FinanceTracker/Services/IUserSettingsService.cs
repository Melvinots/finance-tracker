
namespace FinanceTracker.Services
{
    public interface IUserSettingsService
    {
        Task<UserSettingsDto> GetAsync(int userId);
        Task<UserSettingsDto> UpdateAsync(SaveUserSettingsDto dto, int userId);
    }
}
