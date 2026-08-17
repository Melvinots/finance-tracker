using FinanceTracker.Data;
using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly AppDbContext _context;

        public BudgetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Budget>> GetAllByUserAsync(int userId, int month, int year)
        {
            return await _context.Budgets
                    .Where(b =>
                        b.UserId == userId &&
                        b.Month == month &&
                        b.Year == year)
                    .Include(b => b.Category)
                    .Select(b => new Budget
                    {
                        Id = b.Id,
                        LimitAmount = b.LimitAmount,
                        Month = b.Month,
                        Year = b.Year,
                        UserId = b.UserId,
                        CategoryId = b.CategoryId,
                        Category = b.Category,
                        AmountSpent = b.Category.Transactions
                            .Where(t =>
                                t.UserId == userId &&
                                t.IsExpense &&
                                t.Date.Month == month &&
                                t.Date.Year == year)
                            .Sum(t => t.Amount)
                    })
                    .ToListAsync();
        }

        public async Task<Budget?> GetByIdAsync(int id, int userId)
        {
            return await _context.Budgets
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b =>
                    b.Id == id &&
                    b.UserId == userId);
        }

        public async Task<Budget> CreateAsync(Budget budget)
        {
            await _context.Budgets.AddAsync(budget);
            await _context.SaveChangesAsync();

            return budget;
        }

        public async Task<Budget> UpdateAsync(Budget budget)
        {
            _context.Budgets.Update(budget);
            await _context.SaveChangesAsync();

            return budget;
        }

        public async Task DeleteAsync(Budget budget)
        {
            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
        }
    }
}
