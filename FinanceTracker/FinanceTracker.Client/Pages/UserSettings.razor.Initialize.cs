
namespace FinanceTracker.Client.Pages
{
    public partial class UserSettings
    {
        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            try
            {
                await LoadUserSettingsAsync();
            }
            catch (Exception ex)
            {
                AppNotifier.Error(summary: "Failed to load user settings", ex: ex);
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task LoadUserSettingsAsync()
        {
            _userSettings = await UserSettingsService.GetUserSettingsAsync();
        }
    }
}
