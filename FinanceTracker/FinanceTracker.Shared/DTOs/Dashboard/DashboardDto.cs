using FinanceTracker.Shared.DTOs.UserSettings;

namespace FinanceTracker.Shared.DTOs.Dashboard
{
    public class DashboardDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetSavings => TotalIncome - TotalExpenses;
        public int TransactionCount { get; set; }
        public UserSettingsDto UserSettings { get; set; } = new();
        public List<MonthlySpendingDto> MonthlySpending { get; set; } = new();
        public List<CategorySpendingDto> CategorySpending { get; set; } = new();
        public List<RecentTransactionDto> RecentTransactions { get; set; } = new();
        public List<BudgetProgressDto> Budgets { get; set; } = new();
    }
}
