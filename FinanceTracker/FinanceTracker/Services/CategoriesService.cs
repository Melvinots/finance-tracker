using FinanceTracker.Models;
using FinanceTracker.Repositories;
using FinanceTracker.Shared.DTOs.Categories;

namespace FinanceTracker.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly ICategoriesRepository _repo;

        public CategoriesService(ICategoriesRepository repo)
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

        public async Task<CategoryDto> CreateAsync(SaveCategoryDto dto, int userId)
        {
            var category = new Category
            {
                Name = dto.Name,
                Color = dto.Color,
                Icon = dto.Icon,
                UserId = userId
            };

            var created = await _repo.CreateAsync(category);

            var result = await _repo.GetByIdAsync(created.Id, userId);
            return MapToDto(result!);
        }

        public async Task<CategoryDto> UpdateAsync(int id, SaveCategoryDto dto, int userId)
        {
            var category = await _repo.GetByIdAsync(id, userId)
                ?? throw new KeyNotFoundException("Category not found.");

            category.Name = dto.Name;
            category.Color = dto.Color;
            category.Icon = dto.Icon;

            await _repo.UpdateAsync(category);

            var result = await _repo.GetByIdAsync(id, userId);
            return MapToDto(result!);
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var category = await _repo.GetByIdAsync(id, userId)
                ?? throw new KeyNotFoundException("Category not found.");

            await _repo.DeleteAsync(category);
        }

        // ── private helpers ──────────────────────────────────────────

        private static CategoryDto MapToDto(Category t) => new()
        {
            Id = t.Id,
            Name = t.Name,
            Icon = t.Icon ?? string.Empty,
            Color = t.Color ?? string.Empty,
        };
    }
}
