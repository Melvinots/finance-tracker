using FinanceTracker.Shared.DTOs.Dashboard;

namespace FinanceTracker.Repositories.Dashboard
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
            var transactions = await GetMonthlyTransactionsAsync(userId, month, year);

            var (totalIncome, totalExpenses) = CalculateTotals(transactions);

            var categorySpending = GetCategorySpending(transactions);

            var budgetProgress = await GetBudgetProgressAsync(userId, month, year);

            var recentTransactions = GetRecentTransactions(transactions);

            var monthlySpending = await GetMonthlySpendingAsync(userId, year);

            var userSettings = await GetUserSettingsAsync(userId);

            return new DashboardDto
            {
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                TransactionCount = transactions.Count,
                CategorySpending = categorySpending,
                RecentTransactions = recentTransactions,
                Budgets = budgetProgress,
                MonthlySpending = monthlySpending,
                UserSettings = userSettings
            };
        }

        private async Task<List<Transaction>> GetMonthlyTransactionsAsync(int userId, int month, int year)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t =>
                    t.UserId == userId &&
                    t.Date.Month == month &&
                    t.Date.Year == year)
                .ToListAsync();
        }

        private (decimal TotalIncome, decimal TotalExpenses) CalculateTotals(List<Transaction> transactions)
        {
            var totalIncome = transactions
                .Where(t => !t.IsExpense)
                .Sum(t => t.Amount);

            var totalExpenses = transactions
                .Where(t => t.IsExpense)
                .Sum(t => t.Amount);

            return (totalIncome, totalExpenses);
        }

        private List<CategorySpendingDto> GetCategorySpending(List<Transaction> transactions)
        {
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

            var totalExpenseAmount = categorySpending.Sum(c => c.Amount);

            foreach (var category in categorySpending)
            {
                category.Percentage = totalExpenseAmount > 0
                            ? category.Amount / totalExpenseAmount * 100
                            : 0;
            }

            return categorySpending;
        }

        private async Task<List<BudgetProgressDto>> GetBudgetProgressAsync(int userId, int month, int year)
        {
            var spendingByCategory = await _context.Transactions
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

            var budgets = await _context.Budgets
                .Where(b =>
                    b.UserId == userId &&
                    b.Month == month &&
                    b.Year == year)
                .Include(b => b.Category)
                .ToListAsync();

            return budgets
                .Select(b =>
                {
                    var spent = spendingByCategory
                        .FirstOrDefault(
                            s => s.CategoryId == b.CategoryId)
                        ?.Spent ?? 0;

                    return new BudgetProgressDto
                    {
                        Category = b.Category.Name,
                        Spent = spent,
                        Limit = b.LimitAmount
                    };
                })
                .OrderByDescending(b =>
                    b.Limit > 0
                        ? b.Spent / b.Limit
                        : decimal.MaxValue)
                .Take(5)
                .ToList();
        }

        private List<RecentTransactionDto> GetRecentTransactions(List<Transaction> transactions)
        {
            return transactions
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
        }

        private async Task<List<MonthlySpendingDto>> GetMonthlySpendingAsync(int userId, int year)
        {
            var yearlyTransactions = await _context.Transactions
                .Where(t =>
                    t.UserId == userId &&
                    t.Date.Year == year)
                .ToListAsync();

            return Enumerable.Range(1, 12)
                .Select(month =>
                {
                    var monthTransactions = yearlyTransactions
                        .Where(t => t.Date.Month == month);

                    return new MonthlySpendingDto
                    {
                        MonthNumber = month,
                        Year = year,
                        Month = new DateTime(year, month, 1)
                            .ToString("MMM"),

                        Income = monthTransactions
                            .Where(t => !t.IsExpense)
                            .Sum(t => t.Amount),

                        Expenses = monthTransactions
                            .Where(t => t.IsExpense)
                            .Sum(t => t.Amount)
                    };
                })
                .ToList();
        }

        private async Task<UserSettingsDto> GetUserSettingsAsync(int userId)
        {
            return await _context.UserSettings
                .Where(us => us.UserId == userId)
                .Select(us => new UserSettingsDto
                {
                    Id = us.Id,
                    FullName = us.User.FullName,
                    Email = us.User.Email,
                    Currency = us.Currency,
                    Appearance = us.Appearance
                })
                .FirstAsync();
        }
    }
}
