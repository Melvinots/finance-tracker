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
            var response = await _http.GetAsync(ApiRoutes.UserSettings);
            return await response.ReadOrThrowAsync<UserSettingsDto>();
        }

        public async Task<UserSettingsDto> UpdateUserSettingsAsync(UserSettingsDto dto)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.UserSettings, dto);
            return await response.ReadOrThrowAsync<UserSettingsDto>();
        }

        public async Task DownloadExportAsync()
        {
            var response = await _http.GetAsync(ApiRoutes.AccountExport);
            await response.EnsureSuccessOrThrowAsync();

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var fileName = $"financetracker_export_{DateTime.UtcNow:yyyyMMdd}.csv";

            await _jsRuntime.InvokeVoidAsync("downloadFile", fileName, Convert.ToBase64String(bytes));
        }

        public async Task DeactivateAccountAsync()
        {
            var response = await _http.DeleteAsync(ApiRoutes.Account);
            await response.EnsureSuccessOrThrowAsync();
        }
    }
}
