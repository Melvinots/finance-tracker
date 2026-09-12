using FinanceTracker.Services.Dashboard;
using FinanceTracker.Shared.DTOs.Dashboard;

namespace FinanceTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("GetDashboard")]
        public async Task<ActionResult<DashboardDto>> GetDashboard([FromQuery] int month, [FromQuery] int year)
        {
            var userId = GetUserId();
            var dashboard = await _dashboardService.GetDashboardAsync(userId, month, year);

            return Ok(dashboard);
        }

        // ── private helpers ──────────────────────────────────────────

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}

