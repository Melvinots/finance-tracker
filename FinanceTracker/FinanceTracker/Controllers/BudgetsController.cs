using FinanceTracker.Services;
using FinanceTracker.Shared.DTOs.Budgets;
using FinanceTracker.Shared.DTOs.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BudgetsController : Controller
    {
        private readonly IBudgetService _service;

        public BudgetsController(IBudgetService service)
        {
            _service = service;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<BudgetDto>>> GetAll([FromQuery] int month, [FromQuery] int year)
        {
            var userId = GetUserId();
            var result = await _service.GetAllByUserAsync(userId, month, year);

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
        public async Task<IActionResult> Create(SaveBudgetDto dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await _service.CreateAsync(dto, userId);

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, SaveBudgetDto dto)
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
