using System;
using System.Collections.Generic;
using System.Text;

namespace FinanceTracker.Shared.DTOs.Dashboard
{
    public class RecentTransactionDto
    {
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsIncome { get; set; }
        public DateTime Date { get; set; }
    }

}
