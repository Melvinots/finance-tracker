using FinanceTracker.Models;
using FinanceTracker.Repositories;
using FinanceTracker.Shared.DTOs.Budgets;

namespace FinanceTracker.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _repo;

        public BudgetService(IBudgetRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<BudgetDto>> GetAllByUserAsync(int userId, int month, int year)
        {
            var budgets = await _repo.GetAllByUserAsync(userId, month, year);
            return budgets.Select(MapToDto).ToList();
        }

        public async Task<BudgetDto?> GetByIdAsync(int id, int userId)
        {
            var budget = await _repo.GetByIdAsync(id, userId);

            return budget is null
                ? null
                : MapToDto(budget);
        }

        public async Task<BudgetDto> CreateAsync(
            SaveBudgetDto dto,
            int userId)
        {
            var budget = new Budget
            {
                LimitAmount = dto.LimitAmount,
                Month = dto.Month,
                Year = dto.Year,
                CategoryId = dto.CategoryId,
                UserId = userId
            };

            var created = await _repo.CreateAsync(budget);

            var result = await _repo.GetByIdAsync(
                created.Id,
                userId);

            return MapToDto(result!);
        }

        public async Task<BudgetDto> UpdateAsync(
            int id,
            SaveBudgetDto dto,
            int userId)
        {
            var budget = await _repo.GetByIdAsync(id, userId)
                ?? throw new KeyNotFoundException("Budget not found.");

            budget.LimitAmount = dto.LimitAmount;
            budget.Month = dto.Month;
            budget.Year = dto.Year;
            budget.CategoryId = dto.CategoryId;

            await _repo.UpdateAsync(budget);

            var result = await _repo.GetByIdAsync(id, userId);

            return MapToDto(result!);
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var budget = await _repo.GetByIdAsync(id, userId)
                ?? throw new KeyNotFoundException("Budget not found.");

            await _repo.DeleteAsync(budget);
        }

        // ── private helpers ──────────────────────────────────────────

        private static BudgetDto MapToDto(Budget budget) => new()
        {
            Id = budget.Id,
            LimitAmount = budget.LimitAmount,
            Month = budget.Month,
            Year = budget.Year,
            CategoryId = budget.CategoryId,
            CategoryName = budget.Category?.Name ?? string.Empty,
            CategoryColor = budget.Category?.Color,
            CategoryIcon = budget.Category?.Icon
        };
    }
}
