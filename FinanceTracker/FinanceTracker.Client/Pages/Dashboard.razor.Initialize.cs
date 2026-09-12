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
                Console.WriteLine($"Failed to load dashboard: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }
    }
}
