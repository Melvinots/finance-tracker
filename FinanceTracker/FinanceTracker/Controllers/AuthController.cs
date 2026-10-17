using FinanceTracker.Services;
using FinanceTracker.Shared.DTOs.Auth;

namespace FinanceTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            await _authService.ForgotPasswordAsync(dto);
            return Ok("If that email exists, a reset link has been sent.");
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke(RefreshTokenDto dto)
        {
            await _authService.RevokeTokenAsync(dto.RefreshToken);
            return NoContent();
        }
    }
}
