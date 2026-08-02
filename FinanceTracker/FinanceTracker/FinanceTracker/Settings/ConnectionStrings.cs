namespace FinanceTracker.Settings
{
    public class ConnectionStringSettings
    {
        public const string SectionName = "ConnectionStrings";

        public string DefaultConnection { get; set; } = string.Empty;
    }
}
