using System.Globalization;
using System.Text;

namespace FinanceTracker.Helpers
{
    public static class CsvExporter
    {
        public static string BuildTransactionsCsv(List<Transaction> transactions)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Date,Description,Category,Type,Amount,Notes");

            foreach (var t in transactions)
            {
                sb.AppendLine(string.Join(",",
                    t.Date.ToString("yyyy-MM-dd"),
                    EscapeCsvField(t.Description),
                    EscapeCsvField(t.Category?.Name ?? "Uncategorized"),
                    t.IsExpense ? "Expense" : "Income",
                    t.Amount.ToString(CultureInfo.InvariantCulture),
                    EscapeCsvField(t.Notes ?? "")
                ));
            }

            return sb.ToString();
        }

        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field)) return string.Empty;

            if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }

            return field;
        }
    }
}
