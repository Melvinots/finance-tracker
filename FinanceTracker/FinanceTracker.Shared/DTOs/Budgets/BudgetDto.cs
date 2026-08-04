using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Shared.DTOs.Budgets
{
    public class BudgetDto
    {
        public int Id { get; set; }
        public decimal LimitAmount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? CategoryColor { get; set; }
        public string? CategoryIcon { get; set; }
        public decimal AmountSpent { get; set; }
        public decimal RemainingAmount => LimitAmount - AmountSpent;
        public decimal Percentage => 
            LimitAmount > 0 ? Math.Min(AmountSpent / LimitAmount * 100, 100) : 0;
    }
}
