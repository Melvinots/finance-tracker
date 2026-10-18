using FinanceTracker.Services;
using FinanceTracker.Shared.DTOs.Transactions;

namespace FinanceTracker.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/transactions")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionsController(ITransactionService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TransactionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TransactionDto>>> GetAll()
        {
            var userId = GetUserId();
            var result = await _service.GetAllByUserAsync(userId);

            return Ok(result);
        }

        [HttpGet("{id:int}", Name = "GetTransactionById")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionDto>> GetById(int id)
        {
            var userId = GetUserId();
            var result = await _service.GetByIdAsync(id, userId);

            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TransactionDto>> Create([FromBody] SaveTransactionDto dto)
        {
            var userId = GetUserId();
            var result = await _service.CreateAsync(dto, userId);

            return CreatedAtRoute("GetTransactionById", new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionDto>> Update(int id, [FromBody] SaveTransactionDto dto)
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
