
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Shared.DTOs.Budgets
{
    public class SaveBudgetDto
    {
        [Required(ErrorMessage = "Monthly Limit is required.")]
        [Range(0.01, 10_000_000, ErrorMessage = "Monthly Limit must be greater than zero.")]
        public decimal? LimitAmount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int CategoryId { get; set; }
    }
}
