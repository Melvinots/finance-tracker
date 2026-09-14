namespace FinanceTracker.Shared.DTOs.Transactions
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = default!;
        public DateTime Date { get; set; }
        public bool IsExpense { get; set; }
        public string? Notes { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = default!;
        public string CategoryColor { get; set; } = default!;
        public string CategoryIcon { get; set; } = default!;
    }
}
