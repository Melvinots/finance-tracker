using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.JSInterop;

namespace FinanceTracker.Client.Services
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _js;
        private static readonly AuthenticationState _anonymous =
            new(new ClaimsPrincipal(new ClaimsIdentity()));

        private AuthResponseDto? _currentUser;
        public string? AccessToken => _currentUser?.AccessToken;
        public string? RefreshToken => _currentUser?.RefreshToken;

        public AuthStateProvider(IJSRuntime js)
        {
            _js = js;
        }

        public async Task NotifyUserLoginAsync(AuthResponseDto user)
        {
            _currentUser = user;
            await _js.InvokeVoidAsync("localStorage_set", "auth", JsonSerializer.Serialize(user));

            NotifyAuthenticationStateChanged(Task.FromResult(BuildAuthState(user.AccessToken)));
        }

        public async Task NotifyUserLogoutAsync()
        {
            _currentUser = null;
            await _js.InvokeVoidAsync("localStorage_remove", "auth");

            NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (_currentUser is not null)
                return BuildAuthState(_currentUser.AccessToken);

            try
            {
                var stored = await _js.InvokeAsync<string?>("localStorage_get", "auth");

                if (string.IsNullOrEmpty(stored))
                    return _anonymous;

                var user = JsonSerializer.Deserialize<AuthResponseDto>(stored);
                if (user is null)
                    return _anonymous;

                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(user.AccessToken);
                if (jwt.ValidTo < DateTime.UtcNow)
                {
                    await _js.InvokeVoidAsync("localStorage_remove", "auth");
                    return _anonymous;
                }

                _currentUser = user;
                return BuildAuthState(user.AccessToken);
            }
            catch
            {
                return _anonymous;
            }
        }

        // ── private | public helpers ──────────────────────────────────────────

        private static AuthenticationState BuildAuthState(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwt.Claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            if (_currentUser is not null)
            {
                return _currentUser.AccessToken;
            }

            var stored = await _js.InvokeAsync<string?>("localStorage_get", "auth");

            if (string.IsNullOrWhiteSpace(stored))
            {
                return null;
            }

            var user = JsonSerializer.Deserialize<AuthResponseDto>(stored);

            if (user is null)
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(user.AccessToken);

            if (jwt.ValidTo <= DateTime.UtcNow)
            {
                return null;
            }

            _currentUser = user;

            return _currentUser.AccessToken;
        }
    }
}
