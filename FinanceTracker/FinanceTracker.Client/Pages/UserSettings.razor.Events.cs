
namespace FinanceTracker.Client.Pages
{
    public partial class UserSettings
    {
        private async Task OnPreferenceChangedAsync()
        {
            _isSaving = true;

            try
            {
                await UserSettingsService.UpdateUserSettingsAsync(_userSettings.Id, _userSettings);
                
                await Task.Delay(1000);
                AppNotifier.Success(summary: "Settings saved successfully");
            }
            catch (Exception ex)
            {
                AppNotifier.Error(summary: "Failed to save settings", ex: ex);
            }
            finally
            {
                _isSaving = false;
            }
        }

        private async Task OnExportClick()
        {
            _isExporting = true;

            try
            {
                await UserSettingsService.DownloadExportAsync();

                AppNotifier.Success(summary: "Data exported successfully");
            }
            catch (Exception ex)
            {
                AppNotifier.Error(summary: "Failed to export data", ex: ex);
            }
            finally
            {
                _isExporting = false;
            }
        }

        private async Task OnDeactivateClick()
        {
            bool? confirmed = await AppDialogs.ConfirmDeactivate();

            if (confirmed == true)
            {
                _isDeactivating = true;
                StateHasChanged();

                try
                {
                    await UserSettingsService.DeactivateAccountAsync();
                    AppNotifier.Success(summary: "Account deactivated successfully");

                    await Task.Delay(1000);
                    await AuthService.LogoutAsync();
                    Navigation.NavigateTo("/", forceLoad: true);
                }
                catch (Exception ex)
                {
                    AppNotifier.Error(summary: "Failed to deactivate account", ex: ex);
                }
                finally
                {
                    _isDeactivating = false;
                }
            }
        }
    }
}
