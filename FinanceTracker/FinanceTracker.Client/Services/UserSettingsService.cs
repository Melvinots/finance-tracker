using FinanceTracker.Shared.DTOs.UserSettings;
using Microsoft.JSInterop;

namespace FinanceTracker.Client.Services
{
    public class UserSettingsService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _jsRuntime;

        public UserSettingsService(HttpClient http, IJSRuntime jsRuntime)
        {
            _http = http;
            _jsRuntime = jsRuntime;
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

        public async Task DownloadExportAsync()
        {
            var response = await _http.GetAsync("api/UserSettings/Export");
            response.EnsureSuccessStatusCode();

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var fileName = $"financetracker_export_{DateTime.UtcNow:yyyyMMdd}.csv";

            await _jsRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(bytes));
        }
    }
}
