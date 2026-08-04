using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Shared.DTOs.Budgets
{
    public class SaveBudgetDto
    {
        public decimal LimitAmount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int CategoryId { get; set; }
    }
}
