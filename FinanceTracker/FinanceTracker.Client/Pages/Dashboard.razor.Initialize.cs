
using Microsoft.AspNetCore.WebUtilities;

namespace FinanceTracker.Client.Pages
{
    public partial class Dashboard
    {
        protected override async Task OnInitializedAsync()
        {
            CheckReactivationFlag();
            await LoadDashboardAsync();           
        }

        private async Task LoadDashboardAsync()
        {
            _isLoading = true;

            try
            {
                var selectedDate = _selectedMonth;

                _dashboard = await DashboardService.GetDashboardAsync(selectedDate.Month, selectedDate.Year);
                _currencyCode = _dashboard.UserSettings.Currency ?? string.Empty;
            }
            catch (Exception ex)
            {
                AppNotifier.Error(summary: "Failed to load dashboard", detail: "Something went wrong while loading your data.", ex);
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void CheckReactivationFlag()
        {
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var query = QueryHelpers.ParseQuery(uri.Query);

            if (query.TryGetValue("reactivated", out var value) && value == "true")
            {
                AppNotifier.Info(summary: "Welcome back! Your account has been reactivated.");
                Navigation.NavigateTo("/dashboard", replace: true);
            }
        }

        private async Task OnCurrentDateChanged(DateTime args)
        {
            _selectedMonth = new DateTime(args.Year, args.Month, 1);
            await LoadDashboardAsync();
        }
    }
}
