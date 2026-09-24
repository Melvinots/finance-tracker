using FinanceTracker.Models;
using FinanceTracker.Repositories;
using FinanceTracker.Shared.DTOs.Categories;

namespace FinanceTracker.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<CategoryDto>> GetAllByUserAsync(int userId)
        {
            var categories =  await _repo.GetAllByUserAsync(userId);
            return categories.Select(MapToDto).ToList();
        }

        public async Task<CategoryDto?> GetByIdAsync(int id, int userId)
        {
            var category = await _repo.GetByIdAsync(id, userId);
            return category is null ? null : MapToDto(category);
        }

        public async Task<int> GetTransactionCountAsync(int id, int userId)
        {
            return await _repo.GetTransactionCountAsync(id, userId);
        }

        public async Task<CategoryDto> CreateAsync(SaveCategoryDto dto, int userId)
        {
            var category = new Category
            {
                Name = dto.Name,
                Color = dto.Color,
                Icon = dto.Icon,
                UserId = userId
            };

            var result = await _repo.CreateAsync(category);
            return MapToDto(result);
        }

        public async Task<CategoryDto> UpdateAsync(int id, SaveCategoryDto dto, int userId)
        {
            var category = await _repo.GetByIdAsync(id, userId)
                ?? throw new KeyNotFoundException("Category not found.");

            category.Name = dto.Name;
            category.Color = dto.Color;
            category.Icon = dto.Icon;

            var result = await _repo.UpdateAsync(category);
            return MapToDto(result!);
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var category = await _repo.GetByIdAsync(id, userId)
                ?? throw new KeyNotFoundException("Category not found.");

            if (category.IsSystemDefault)
                throw new InvalidOperationException("This category cannot be deleted.");

            var fallbackCategory = await _repo.GetSystemDefaultAsync(userId)
                ?? throw new InvalidOperationException("Fallback category not found");
            
            await _repo.DeleteWithReassignmentAsync(category, fallbackCategory.Id, userId);
        }

        // ── private helpers ──────────────────────────────────────────

        private static CategoryDto MapToDto(Category t) => new()
        {
            Id = t.Id,
            Name = t.Name,
            Icon = t.Icon ?? string.Empty,
            Color = t.Color ?? string.Empty,
            IsSystemDefault = t.IsSystemDefault
        };
    }
}
