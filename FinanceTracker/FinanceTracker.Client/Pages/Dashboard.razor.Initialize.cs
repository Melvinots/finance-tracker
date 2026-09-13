using Radzen;

namespace FinanceTracker.Client.Pages
{
    public partial class Dashboard
    {
        protected override async Task OnInitializedAsync()
        {
            await LoadDashboardAsync();
        }

        private async Task LoadDashboardAsync()
        {
            _isLoading = true;

            try
            {
                var selectedDate = DateTime.ParseExact(_selectedMonth, "yyyy-MM", null);
                
                _dashboard = await DashboardService.GetDashboardAsync(selectedDate.Month, selectedDate.Year);
                _currencyCode = _dashboard.UserSettings.Currency ?? string.Empty;
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Failed to load dashboard",
                    Detail = "Something went wrong while loading your data.",
                    Duration = 4000
                });

                Console.WriteLine($"Failed to load dashboard: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }
    }
}
