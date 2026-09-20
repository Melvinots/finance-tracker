namespace FinanceTracker.Client.Helpers
{
    public static class CurrencyFormatter
    {
        public static string Format(decimal amount, string currencyCode)
        {
            var decimals = GetDecimalPlaces(currencyCode);
            var currencySymbol = currencyCode switch
            {
                "PHP" => "₱",
                "USD" => "$",
                "EUR" => "€",
                "JPY" => "¥",
                "GBP" => "£",
                _ => currencyCode
            };

            var rounded = Math.Round(amount, decimals, MidpointRounding.AwayFromZero);

            return $"{currencySymbol}{rounded.ToString($"N{decimals}")}";
        }

        public static int GetDecimalPlaces(string currencyCode) => currencyCode switch
        {
            "JPY" => 0,
            _ => 2
        };
    }
}
