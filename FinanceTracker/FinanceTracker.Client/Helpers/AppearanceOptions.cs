namespace FinanceTracker.Client.Helpers
{
    public static class AppearanceOptions
    {
        public static readonly IReadOnlyList<(string Code, string DisplayName)> All = new List<(string, string)>
        {
            ("light", "Light"),
            ("dark", "Dark"),
            ("system", "System"),
        };
    }
}
