namespace FinanceTracker.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsExpense { get; set; } = true;
        public string? Notes { get; set; }

        public int UserId { get; set; }
        public int CategoryId { get; set; }

        public User User { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}
