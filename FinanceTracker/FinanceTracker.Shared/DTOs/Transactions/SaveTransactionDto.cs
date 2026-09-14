using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Shared.DTOs.Transactions
{
    public class SaveTransactionDto
    {
        [Required]
        [Range(0.01, 10_000_000, ErrorMessage = "Amount must be greater than zero.")]
        public decimal? Amount { get; set; } = null;

        [Required]
        [MaxLength(100)]
        public string Description { get; set; } = default!;

        [Required]
        public DateTime Date { get; set; } = DateTime.Today;

        public bool IsExpense { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }

        public int CategoryId { get; set; } = 0;
    }
}
