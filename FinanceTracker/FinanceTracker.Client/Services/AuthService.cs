using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly AuthStateProvider _authStateProvider;
        private string? _refreshToken;

        public AuthService(HttpClient http, AuthStateProvider authStateProvider)
        {
            _http = http;
            _authStateProvider = authStateProvider;
        }

        public async Task<string?> RegisterAsync(RegisterDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/Auth/Register", dto);

            if (!response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            _refreshToken = result!.RefreshToken;
            await _authStateProvider.NotifyUserLoginAsync(result!);
            return null;
        }

        public async Task<LoginResult> LoginAsync(LoginDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/Auth/Login", dto);

            if (!response.IsSuccessStatusCode)
            {
                var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                return new LoginResult(Success: false, Error: problem?.Detail ?? "Login failed.", WasReactivated: false);
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>()
                ?? throw new InvalidOperationException("Login succeeded but response was empty.");

            _refreshToken = result.RefreshToken;
            await _authStateProvider.NotifyUserLoginAsync(result);

            return new LoginResult(Success: true, Error: null, WasReactivated: result.IsReactivated);
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _http.PostAsJsonAsync("api/Auth/Revoke",
                    _authStateProvider.RefreshToken);
            }
            catch { }
            finally
            {
                await _authStateProvider.NotifyUserLogoutAsync();
            }
        }
    }
}
