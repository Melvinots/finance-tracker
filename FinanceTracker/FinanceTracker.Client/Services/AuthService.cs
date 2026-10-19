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
            var response = await _http.PostAsJsonAsync(ApiRoutes.AuthRegister, dto);
            var (result, error) = await response.TryReadAsync<AuthResponseDto>();

            if (error is not null)
                return error;

            await _authStateProvider.NotifyUserLoginAsync(result!);
            return null;
        }

        public async Task<LoginResult> LoginAsync(LoginDto dto)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.AuthLogin, dto);
            var (result, error) = await response.TryReadAsync<AuthResponseDto>();

            if (error is not null)
                return new LoginResult(Success: false, Error: error, WasReactivated: false);

            await _authStateProvider.NotifyUserLoginAsync(result!);
            return new LoginResult(Success: true, Error: null, WasReactivated: result!.IsReactivated);
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _http.PostAsJsonAsync(ApiRoutes.AuthRevoke, _authStateProvider.RefreshToken);
            }
            catch { }
            finally
            {
                await _authStateProvider.NotifyUserLogoutAsync();
            }
        }
    }
}
