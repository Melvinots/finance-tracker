namespace FinanceTracker.Client.Helpers
{
    public static class CurrencyFormatter
    {
        public static string Format(decimal amount, string currencyCode)
        {
            var currencySymbol = currencyCode switch
            {
                "PHP" => "₱",
                "USD" => "$",
                "EUR" => "€",
                "JPY" => "¥",
                "GBP" => "£",
                _ => currencyCode
            };

            return $"{currencySymbol} {amount:N0}";
        }
    }
}
