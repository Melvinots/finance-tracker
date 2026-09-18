using FinanceTracker.Shared.DTOs.UserSettings;

namespace FinanceTracker.Client.Pages
{
    public partial class UserSettings
    {
        [Inject] UserSettingsService UserSettingsService { get; set; } = default!;
        [Inject] AppNotificationService AppNotifier { get; set; } = default!;

        private bool _isLoading = true;

        private bool _isSaving;

        private bool _isExporting;

        private bool _isDeactivating;

        private UserSettingsDto _userSettings = new();
    }
}