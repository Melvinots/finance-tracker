using FinanceTracker.Shared.DTOs.UserSettings;
using Microsoft.JSInterop;

namespace FinanceTracker.Client.Pages
{
    public partial class UserSettings
    {
        [Inject] UserSettingsService UserSettingsService { get; set; } = default!;
        [Inject] AppNotificationService AppNotifier { get; set; } = default!;
        [Inject] AppDialogService AppDialogs { get; set; } = default!;
        [Inject] AuthService AuthService { get; set; } = default!;
        [Inject] NavigationManager Navigation { get; set; } = default!;

        private bool _isLoading = true;
        private bool _isSaving;
        private bool _isExporting;
        private bool _isDeactivating;
        private UserSettingsDto _userSettings = new();
    }
}