using FinanceTracker.Shared.DTOs.UserSettings;

namespace FinanceTracker.Client.Services
{
    public class UserSettingsService
    {
        private readonly HttpClient _http;

        public UserSettingsService(HttpClient http)
        {
            _http = http;
        }

        public async Task<UserSettingsDto> GetUserSettingsAsync()
        {
            return await _http.GetFromJsonAsync<UserSettingsDto>("api/UserSettings/GetUserSettings")
                ?? new UserSettingsDto();
        }

        public async Task UpdateUserSettingsAsync(int id, UserSettingsDto dto)
        {
            var response = await _http.PutAsJsonAsync($"api/UserSettings/UpdateUserSettings/{id}", dto);
            response.EnsureSuccessStatusCode();
        }
    }
}
