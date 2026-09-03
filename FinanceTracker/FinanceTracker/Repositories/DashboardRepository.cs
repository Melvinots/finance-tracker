using FinanceTracker.Data;
using FinanceTracker.Shared.DTOs.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly AppDbContext _context;

        public DashboardRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetDashboardAsync(int userId, int month, int year)
        {
            var transactions = await _context.Transactions
                .Include(t => t.Category)
                .Where(t =>
                    t.UserId == userId &&
                    t.Date.Month == month &&
                    t.Date.Year == year)
                .ToListAsync();

            var totalIncome = transactions
                .Where(t => !t.IsExpense)
                .Sum(t => t.Amount);

            var totalExpenses = transactions
                .Where(t => t.IsExpense)
                .Sum(t => t.Amount);

            var categorySpending = transactions
                .Where(t => t.IsExpense)
                .GroupBy(t => new
                {
                    t.CategoryId,
                    CategoryName = t.Category?.Name,
                    CategoryColor = t.Category?.Color
                })
                .Select(group => new CategorySpendingDto
                {
                    Name = group.Key.CategoryName ?? "Uncategorized",
                    Color = group.Key.CategoryColor ?? "#94A3B8",
                    Amount = group.Sum(t => t.Amount)
                })
                .ToList();

            var totalExpenseAmount = categorySpending
                .Sum(c => c.Amount);

            foreach (var category in categorySpending)
            {
                category.Percentage =
                    totalExpenseAmount > 0
                        ? category.Amount / totalExpenseAmount * 100
                        : 0;
            }

            var spendingByCategory =
                await _context.Transactions
                    .Where(t =>
                        t.UserId == userId &&
                        t.IsExpense &&
                        t.Date.Month == month &&
                        t.Date.Year == year)
                    .GroupBy(t => t.CategoryId)
                    .Select(g => new
                    {
                        CategoryId = g.Key,
                        Spent = g.Sum(t => t.Amount)
                    })
                    .ToListAsync();

            var budgets =
                await _context.Budgets
                    .Where(b =>
                        b.UserId == userId &&
                        b.Month == month &&
                        b.Year == year)
                    .Include(b => b.Category)
                    .ToListAsync();

            var budgetProgress =
                budgets.Select(b =>
                {
                    var spent =
                        spendingByCategory
                            .FirstOrDefault(
                                s => s.CategoryId == b.CategoryId
                            )
                            ?.Spent ?? 0;

                    return new BudgetProgressDto
                    {
                        Category = b.Category.Name,
                        Spent = spent,
                        Limit = b.LimitAmount
                    };
                })
                .OrderByDescending(b => 
                    b.Limit > 0 ? 
                    b.Spent / b.Limit : 
                    decimal.MaxValue)
                .Take(5)
                .ToList();

            var recentTransactions = transactions
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.Id)
                .Take(5)
                .Select(t => new RecentTransactionDto
                {
                    Description = t.Description,
                    Category = t.Category?.Name ?? "Uncategorized",
                    Amount = t.Amount,
                    IsIncome = !t.IsExpense,
                    Date = t.Date
                })
                .ToList();

            var yearlyTransactions = await _context.Transactions
                .Where(t =>
                    t.UserId == userId &&
                    t.Date.Year == year)
                .ToListAsync();

            var monthlySpending = Enumerable.Range(1, 12)
                .Select(month =>
                {
                    var monthTransactions = yearlyTransactions
                        .Where(t => t.Date.Month == month);

                    return new MonthlySpendingDto
                    {
                        MonthNumber = month,
                        Year = year,
                        Month = new DateTime(year, month, 1).ToString("MMM"),

                        Income = monthTransactions
                            .Where(t => !t.IsExpense)
                            .Sum(t => t.Amount),

                        Expenses = monthTransactions
                            .Where(t => t.IsExpense)
                            .Sum(t => t.Amount)
                    };
                })
                .ToList();

            return new DashboardDto
            {
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                TransactionCount = transactions.Count,
                CategorySpending = categorySpending,
                RecentTransactions = recentTransactions,
                Budgets = budgetProgress,
                MonthlySpending = monthlySpending
            };
        }
    }
}
