using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Shared.DTOs.Common
{
    public class MonthYearQueryDto
    {
        [Range(1, 12)]
        public int Month { get; set; } = DateTime.UtcNow.Month;

        [Range(2000, 2100)]
        public int Year { get; set; } = DateTime.UtcNow.Year;
    }
}
