using FinanceTracker.Services;
using FinanceTracker.Shared.DTOs.Budgets;

namespace FinanceTracker.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/budgets")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetService _service;

        public BudgetsController(IBudgetService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<BudgetDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<BudgetDto>>> GetAll([FromQuery] BudgetQueryDto dto)
        {
            var userId = GetUserId();
            var result = await _service.GetAllByUserAsync(userId, dto.Month, dto.Year);

            return Ok(result);
        }

        [HttpGet("{id:int}", Name = "GetBudgetById")]
        [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BudgetDto>> GetById(int id)
        {
            var userId = GetUserId();
            var result = await _service.GetByIdAsync(id, userId);

            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BudgetDto>> Create([FromBody] SaveBudgetDto dto)
        {
            var userId = GetUserId();
            var result = await _service.CreateAsync(dto, userId);

            return CreatedAtRoute("GetBudgetById", new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(BudgetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BudgetDto>> Update(int id, [FromBody] SaveBudgetDto dto)
        {
            var userId = GetUserId();
            var result = await _service.UpdateAsync(id, dto, userId);

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            await _service.DeleteAsync(id, userId);

            return NoContent();
        }

        // ── private helpers ──────────────────────────────────────────

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
