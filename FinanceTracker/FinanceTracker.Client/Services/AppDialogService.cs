using Radzen;

namespace FinanceTracker.Client.Services
{
    public class AppDialogService
    {
        private readonly DialogService _dialogService;

        public AppDialogService(DialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public Task<bool?> ConfirmDelete(string itemDescription, string itemType)
        {
            return _dialogService.Confirm(
                $"This will permanently remove \"{itemDescription}\". This can't be undone.",
                $"Delete {itemType}?",
                new ConfirmOptions { OkButtonText = "Delete", CancelButtonText = "Cancel" }
            );
        }
    }
}
