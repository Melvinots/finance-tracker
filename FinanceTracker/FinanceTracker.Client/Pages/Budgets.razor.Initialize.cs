namespace FinanceTracker.Client.Pages
{
    public partial class Budgets
    {
        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            try
            {
                await LoadBudgetsAsync();
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

        private async Task LoadBudgetsAsync()
        {
            _budgets = await BudgetService.GetAllAsync(SelectedDate.Month, SelectedDate.Year);
        }

        private async Task LoadCategoriesAsync()
        {
            _allCategories = await CategoryService.GetAllAsync();
            UpdateAvailableCategories();
        }

        private async Task LoadUserSettingsAsync()
        {
            var userSettings = await UserSettingsService.GetUserSettingsAsync();
            _currencyCode = userSettings.Currency;
        }

        private void UpdateAvailableCategories()
        {
            _categories = _allCategories
                .Where(category => !_budgets.Any(
                    budget => budget.CategoryId == category.Id))
                .ToList();
        }
    }
}
