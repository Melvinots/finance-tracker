using FinanceTracker.Services;

namespace FinanceTracker.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user-settings")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class UserSettingsController : ControllerBase
    {
        private readonly IUserSettingsService _service;

        public UserSettingsController(IUserSettingsService settingsService)
        {
            _service = settingsService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserSettingsDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserSettingsDto>> GetUserSettings()
        {
            var userId = GetUserId();
            var settings = await _service.GetAsync(userId);

            return Ok(settings);
        }

        [HttpPut]
        [ProducesResponseType(typeof(UserSettingsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserSettingsDto>> UpdateUserSettings([FromBody] SaveUserSettingsDto dto)
        {
            var userId = GetUserId();
            var settings = await _service.UpdateAsync(dto, userId);

            return Ok(settings);
        }

        // ── private helpers ──────────────────────────────────────────

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
