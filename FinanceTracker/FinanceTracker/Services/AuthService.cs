using FinanceTracker.Settings;
using FinanceTracker.Shared.DTOs.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace FinanceTracker.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtSettings _config;

        public AuthService(AppDbContext context, IOptions<JwtSettings> jwtOptions)
        {
            _context = context;
            _config = jwtOptions.Value;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var exists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email.ToLower());

            if (exists)
                throw new ConflictException("Email already registered.");

            ValidateEmail(dto.Email);
            ValidatePassword(dto.Password);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                IsActive = true
            };

            var settings = new UserSettings
            {
                User = user,
                Currency = "USD",
                Appearance = "light"
            };

            await _context.Users.AddAsync(user);
            await _context.UserSettings.AddAsync(settings);
            await _context.SaveChangesAsync();

            await SeedDefaultCategoriesAsync(user.Id);

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new AuthFailedException();

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (token is null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
                throw new AuthFailedException("Invalid or expired refresh token.");

            token.IsRevoked = true;
            await _context.SaveChangesAsync();

            return await GenerateAuthResponseAsync(token.User);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

            if (user is null) return;
            
            var resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            //Console.WriteLine($"[DEV] Password reset token for {dto.Email}: {resetToken}");
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (token is not null)
            {
                token.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }

        // ── private helpers ──────────────────────────────────────────

        private async Task<AuthResponseDto> GenerateAuthResponseAsync(User user)
        {
            var accessToken = GenerateAccessToken(user);
            var refreshToken = await GenerateRefreshTokenAsync(user.Id);
            var isReactivated = !user.IsActive;

            if (isReactivated)
            {
                user.IsActive = true;
                await _context.SaveChangesAsync();
            }

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                FullName = user.FullName,
                Email = user.Email,
                IsReactivated = isReactivated
            };
        }

        private string GenerateAccessToken(User user)
        {
            var secret = _config.Secret;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            var expiry = _config.AccessTokenExpiryMinutes;

            var token = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiry),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expiryDays = _config.RefreshTokenExpiryDays;

            var refreshToken = new RefreshToken
            {
                Token = token,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return token;
        }

        private async Task SeedDefaultCategoriesAsync(int userId)
        {
            var exists = await _context.Categories
                .AnyAsync(c => c.UserId == userId && c.Name == "Other");

            if (!exists)
            {
                _context.Categories.Add(new Category
                {
                    UserId = userId,
                    Name = "Other",
                    Icon = "Other",
                    Color = "#64748B",
                    IsSystemDefault = true
                });
                await _context.SaveChangesAsync();
            }
        }

        private void ValidateEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationFailedException("Email is required.", "Email");

            if (!System.Net.Mail.MailAddress.TryCreate(email, out var address) || address.Address != email)
            {
                throw new ValidationFailedException("Please enter a valid email address.", "Email");
            }
        }

        private void ValidatePassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ValidationFailedException("Password is required.", "Password");

            if (password.Length < 8)
                throw new ValidationFailedException("Password must be at least 8 characters long.", "Password");

            if (!password.Any(char.IsUpper))
                throw new ValidationFailedException("Password must contain at least one uppercase letter.", "Password");

            if (!password.Any(char.IsLower))
                throw new ValidationFailedException("Password must contain at least one lowercase letter.", "Password");

            if (!password.Any(char.IsDigit))
                throw new ValidationFailedException("Password must contain at least one number.", "Password");

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                throw new ValidationFailedException("Password must contain at least one special character.", "Password");
        }
    }
}
