using FinanceTracker.Services;
using System.Text;

namespace FinanceTracker.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/account")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountController(IAccountService accountService)
        {
            _service = accountService;
        }

        [HttpGet("export")]
        [Produces("text/csv")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportData()
        {
            var userId = GetUserId();
            var csv = await _service.ExportDataAsync(userId);

            var fileName = $"financetracker_export_{DateTime.UtcNow:yyyyMMdd}.csv";
            var bytes = Encoding.UTF8.GetBytes(csv);

            return File(bytes, "text/csv", fileName);
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeactivateAccount()
        {
            var userId = GetUserId();
            await _service.DeactivateAccountAsync(userId);

            return NoContent();
        }

        // ── private helpers ──────────────────────────────────────────

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
