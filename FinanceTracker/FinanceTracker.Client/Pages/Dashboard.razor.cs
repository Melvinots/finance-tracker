using Radzen;

namespace FinanceTracker.Client.Pages
{
    public partial class Dashboard
    {
        [Inject] private DashboardService DashboardService { get; set; } = default!;
        [Inject] private AppNotificationService AppNotifier { get; set; } = default!;

        private bool _isLoading = true;

        private string _selectedMonth = DateTime.Now.ToString("yyyy-MM");

        private string _currencyCode = string.Empty;

        private DashboardDto _dashboard = new();
    }
}
