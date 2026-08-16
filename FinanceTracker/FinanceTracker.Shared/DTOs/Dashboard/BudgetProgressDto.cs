namespace FinanceTracker.Shared.DTOs.Dashboard
{
    public class BudgetProgressDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Spent { get; set; }
        public decimal Limit { get; set; }
        public decimal Percentage => Limit > 0 ? Spent / Limit * 100 : 0;
        public bool IsOver => Spent > Limit;
    }
}
