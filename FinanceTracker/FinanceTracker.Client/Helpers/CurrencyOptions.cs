namespace FinanceTracker.Client.Helpers
{
    public static class CurrencyOptions
    {
        public static readonly IReadOnlyList<(string Code, string DisplayName)> All = new List<(string, string)>
        {
            ("PHP", "PHP — Philippine Peso"),
            ("USD", "USD — US Dollar"),
            ("EUR", "EUR — Euro"),
            ("JPY", "JPY — Japanese Yen"),
            ("GBP", "GBP — British Pound"),
        };
    }
}
