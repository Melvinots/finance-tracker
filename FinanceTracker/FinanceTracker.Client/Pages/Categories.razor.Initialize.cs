namespace FinanceTracker.Client.Pages
{
    public partial class Categories
    {
        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            try
            {
                await LoadCategoriesAsync();
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

        private async Task LoadCategoriesAsync()
        {
            _categories = await CategoryService.GetAllAsync();
        }
    }
}
