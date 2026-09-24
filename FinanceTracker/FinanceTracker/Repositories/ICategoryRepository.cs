using FinanceTracker.Models;
using FinanceTracker.Shared.DTOs.Categories;

namespace FinanceTracker.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllByUserAsync(int userId);
        Task<Category?> GetByIdAsync(int id, int userId);
        Task<int> GetTransactionCountAsync(int id, int userId);
        Task<Category?> GetSystemDefaultAsync(int userId);
        Task<Category> CreateAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task DeleteWithReassignmentAsync(Category category, int fallbackCategoryId, int userId);
    }
}
