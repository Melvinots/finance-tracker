using FinanceTracker.Shared.DTOs.Auth;

namespace FinanceTracker.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task ForgotPasswordAsync(ForgotPasswordDto dto);
        Task RevokeTokenAsync(string refreshToken);
    }
}
