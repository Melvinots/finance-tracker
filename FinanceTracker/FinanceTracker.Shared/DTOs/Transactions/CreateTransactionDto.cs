using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceTracker.Shared.DTOs.Transactions
{
    public class CreateTransactionDto
    {
        [Required]
        [Range(0.01, 10_000_000, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; } = DateTime.Today;

        public bool IsExpense { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
