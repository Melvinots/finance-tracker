
namespace FinanceTracker.Shared.DTOs.UserSettings
{
    public class UserSettingsDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Currency { get; set; } = "PHP";
        public string Appearance { get; set; } = "light";
    }
}
