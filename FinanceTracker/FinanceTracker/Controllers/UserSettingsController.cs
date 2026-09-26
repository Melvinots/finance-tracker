 using FinanceTracker.Services;
using FinanceTracker.Shared.DTOs.UserSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserSettingsController : Controller
    {
        private readonly IUserSettingsService _service;

        public UserSettingsController(IUserSettingsService settingsService)
        {
            _service = settingsService;
        }

        [HttpGet("GetUserSettings")]
        public async Task<IActionResult> GetUserSettings()
        {
            var userId = GetUserId();
            var settings = await _service.GetAsync(userId);

            return Ok(settings);
        }

        [HttpPut("UpdateUserSettings/{id}")]
        public async Task<IActionResult> UpdateUserSettings(int id, SaveUserSettingsDto dto)
        {
            var userId = GetUserId();
            var settings = await _service.UpdateAsync(id, dto, userId);

            return Ok(settings);
        }

        [HttpGet("Export")]
        public async Task<IActionResult> ExportData()
        {
            var userId = GetUserId();
            var csv = await _service.ExportDataAsync(userId);

            var fileName = $"financetracker_export_{DateTime.UtcNow:yyyyMMdd}.csv";
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);

            return File(bytes, "text/csv", fileName);
        }

        [HttpPost("Deactivate")]
        public async Task<IActionResult> DeactivateAccount()
        {
            var userId = GetUserId();
            await _service.DeactivateAccountAsync(userId);

            return Ok(new { Message = "Account deactivated successfully." });
        }



        // ── private helpers ──────────────────────────────────────────

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
