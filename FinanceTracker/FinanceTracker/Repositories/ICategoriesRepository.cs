using FinanceTracker.Models;
using FinanceTracker.Shared.DTOs.Categories;

namespace FinanceTracker.Repositories
{
    public interface ICategoriesRepository
    {
        Task<List<Category>> GetAllByUserAsync(int userId);
        Task<Category?> GetByIdAsync(int id, int userId);
        Task<Category> CreateAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task DeleteAsync(Category category);
    }
}
