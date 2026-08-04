using FinanceTracker.Shared.DTOs.Budgets;

namespace FinanceTracker.Services
{
    public interface IBudgetService
    {
        Task<List<BudgetDto>> GetAllByUserAsync(int userId, int month, int year);
        Task<BudgetDto?> GetByIdAsync(int id, int userId);
        Task<BudgetDto> CreateAsync(SaveBudgetDto dto, int userId);
        Task<BudgetDto> UpdateAsync(int id, SaveBudgetDto dto, int userId);
        Task DeleteAsync(int id, int userId);
    }
}
