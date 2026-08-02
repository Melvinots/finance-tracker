namespace FinanceTracker.Settings
{
    public class LoggingSettings
    {
        public const string SectionName = "Logging";

        public LogLevelSettings LogLevel { get; set; } = default!;
    }
}
