using FinanceTracker.Models;
using FinanceTracker.Shared.DTOs.Categories;

namespace FinanceTracker.Services
{
    public interface ICategoriesService
    {
        Task<List<CategoryDto>> GetAllByUserAsync(int userId);
        Task<CategoryDto?> GetByIdAsync(int id, int userId);
        Task<CategoryDto> CreateAsync(SaveCategoryDto dto, int userId);
        Task<CategoryDto> UpdateAsync(int id, SaveCategoryDto dto, int userId);
        Task DeleteAsync(int id, int userId);
    }
}
