using FinanceTracker.Services.Dashboard;
using FinanceTracker.Shared.DTOs.Dashboard;

namespace FinanceTracker.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/dashboard")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DashboardDto>> GetDashboard([FromQuery] MonthYearQueryDto dto)
        {
            var userId = GetUserId();
            var dashboard = await _dashboardService.GetDashboardAsync(userId, dto.Month, dto.Year);

            return Ok(dashboard);
        }

        // ── private helpers ──────────────────────────────────────────

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}

