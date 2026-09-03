namespace FinanceTracker.Models
{
    public class UserSettings
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Currency { get; set; } = "PHP";
        public string Appearance { get; set; } = "light";

        public User User { get; set; } = null!;
    }
}
