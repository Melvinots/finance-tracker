using Blazicons;

namespace FinanceTracker.Client.Helpers
{
    public static class Icons
    {
        public static SvgIcon Category(string? name)
        {
            return name switch
            {
                "Shopping" => Lucide.ShoppingCart,
                "Transport" => Lucide.Bus,
                "Utilities" => Lucide.Zap,
                "Entertainment" => Lucide.Film,
                "Health" => Lucide.HeartPulse,
                "Home" => Lucide.House,
                "Clothing" => Lucide.Shirt,
                "Education" => Lucide.BookOpen,
                "Food" => Lucide.Utensils,
                "Travel" => Lucide.Plane,
                "Work" => Lucide.Briefcase,
                "Gaming" => Lucide.Gamepad2,
                "Medicine" => Lucide.Pill,
                "Pets" => Lucide.PawPrint,
                "Gifts" => Lucide.Gift,
                "Other" => Lucide.Inbox,
                _ => Lucide.Inbox
            };
        }
    }
}
