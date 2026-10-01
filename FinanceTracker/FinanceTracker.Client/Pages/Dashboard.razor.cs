using Radzen;

namespace FinanceTracker.Client.Pages
{
    public partial class Dashboard
    {
        [Inject] private DashboardService DashboardService { get; set; } = default!;
        [Inject] private AppNotificationService AppNotifier { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private bool _isLoading = true;

        private DateTime _selectedMonth = DateTime.Now;

        private string _currencyCode = string.Empty;

        private DashboardDto _dashboard = new();
    }
}
