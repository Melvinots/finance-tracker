using FinanceTracker.Data;
using FinanceTracker.Shared.DTOs.Auth;
using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace FinanceTracker.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var exists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email);

            if (exists)
                throw new InvalidOperationException("Email already registered.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email.ToLower());

            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid email or password.");

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (token is null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

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
            Console.WriteLine($"[DEV] Password reset token for {dto.Email}: {resetToken}");
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

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                FullName = user.FullName,
                Email = user.Email
            };
        }

        private string GenerateAccessToken(User user)
        {
            var secret = _config["JwtSettings:Secret"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            var expiry = int.Parse(_config["JwtSettings:AccessTokenExpiryMinutes"]!);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiry),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var expiryDays = int.Parse(_config["JwtSettings:RefreshTokenExpiryDays"]!);

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
    }
}
