namespace FinanceTracker.Shared.DTOs.Dashboard
{
    public class CategorySpendingDto
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }
}
