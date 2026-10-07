using FinanceTracker.Client.Components.Transactions;

namespace FinanceTracker.Client.Pages
{
    public partial class Transactions
    {
        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            try
            {
                await LoadTransactionsAsync();
                await LoadCategoriesAsync();
                await LoadUserSettingsAsync();
            }
            catch (Exception ex)
            {
                AppNotifier.Error(summary: "Failed to load data", ex: ex);
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task LoadTransactionsAsync()
        {
            _transactions = await TransactionService.GetAllAsync();
            ApplyFilters(_filter);
        }

        private async Task LoadCategoriesAsync()
        {
            _categories = await CategoryService.GetAllAsync();
        }

        private async Task LoadUserSettingsAsync()
        {
            var userSettings = await UserSettingsService.GetUserSettingsAsync();
            _currencyCode = userSettings.Currency;
        }
    }
}
