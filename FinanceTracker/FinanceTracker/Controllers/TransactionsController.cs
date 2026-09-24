using FinanceTracker.Services;
using FinanceTracker.Shared.DTOs.Transactions;

namespace FinanceTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionsController(ITransactionService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            var result = await _service.GetAllByUserAsync(userId);

            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();
            var result = await _service.GetByIdAsync(id, userId);

            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(SaveTransactionDto dto)
        {
            var userId = GetUserId();
            var result = await _service.CreateAsync(dto, userId);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, SaveTransactionDto dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await _service.UpdateAsync(id, dto, userId);

                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = GetUserId();
                await _service.DeleteAsync(id, userId);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // ── private helpers ──────────────────────────────────────────

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
