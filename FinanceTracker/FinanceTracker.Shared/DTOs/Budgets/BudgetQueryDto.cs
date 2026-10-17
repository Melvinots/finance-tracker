using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FinanceTracker.Shared.DTOs.Budgets
{
    public class BudgetQueryDto
    {
        [Range(1, 12)]
        public int Month { get; set; }

        [Range(2000, 2100)]
        public int Year { get; set; }
    }

}
