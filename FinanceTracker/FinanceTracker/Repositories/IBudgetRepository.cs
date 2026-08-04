using FinanceTracker.Models;

namespace FinanceTracker.Repositories
{
    public interface IBudgetRepository
    {
        Task<List<Budget>> GetAllByUserAsync(int userId, int month, int year);
        Task<Budget?> GetByIdAsync(int id, int userId);
        Task<Budget> CreateAsync(Budget budget);
        Task<Budget> UpdateAsync(Budget budget);
        Task DeleteAsync(Budget budget);
    }
}
