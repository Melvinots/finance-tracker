using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Shared.DTOs.Dashboard
{
    public class MonthlySpendingDto
    {
        public string Month { get; set; } = string.Empty;
        public int MonthNumber { get; set; }
        public int Year { get; set; }
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
    }
}
