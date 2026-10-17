using FinanceTracker.Services;
using FinanceTracker.Shared.DTOs.Categories;

namespace FinanceTracker.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/categories")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CategoryDto>>> GetAll()
        {
            var userId = GetUserId();
            var result = await _service.GetAllByUserAsync(userId);

            return Ok(result);
        }

        [HttpGet("{id:int}", Name = "GetCategoryById")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var userId = GetUserId();
            var result = await _service.GetByIdAsync(id, userId);

            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet("{id:int}/transaction-count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetTransactionCount(int id)
        {
            var userId = GetUserId();
            var result = await _service.GetTransactionCountAsync(id, userId);

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] SaveCategoryDto dto)
        {
            var userId = GetUserId();
            var result = await _service.CreateAsync(dto, userId);

            return CreatedAtRoute("GetCategoryById", new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] SaveCategoryDto dto)
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
