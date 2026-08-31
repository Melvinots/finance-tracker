namespace FinanceTracker.Shared.DTOs.Dashboard
{
    public class DashboardDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetSavings => TotalIncome - TotalExpenses;
        public int TransactionCount { get; set; }
        public List<MonthlySpendingDto> MonthlySpending { get; set; } = new();
        public List<CategorySpendingDto> CategorySpending { get; set; } = [];
        public List<RecentTransactionDto> RecentTransactions { get; set; } = [];
        public List<BudgetProgressDto> Budgets { get; set; } = [];
    }
}
