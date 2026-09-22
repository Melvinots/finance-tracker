using FinanceTracker.Data;
using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllByUserAsync(int userId)
        {
            return await _context.Categories
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id, int userId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        }
        public async Task<int> GetTransactionCountAsync(int id, int userId)
        {
            return await _context.Transactions
                .CountAsync(t => t.CategoryId == id && t.UserId == userId);
        }

        public async Task<Category> CreateAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task DeleteAsync(int id, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var category = await GetByIdAsync(id, userId);

                if (category is null)
                    throw new KeyNotFoundException("Category not found");

                if (category.IsSystemDefault)
                    throw new InvalidOperationException("This category cannot be deleted.");

                var fallbackCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.IsSystemDefault && c.UserId == userId);

                if (fallbackCategory is null)
                    throw new InvalidOperationException("Fallback category not found");

                await _context.Transactions
                    .Where(t => t.CategoryId == id && t.UserId == userId)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(t => t.CategoryId, fallbackCategory.Id));

                await _context.Budgets
                    .Where(b => b.CategoryId == id && b.UserId == userId)
                    .ExecuteDeleteAsync();

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
