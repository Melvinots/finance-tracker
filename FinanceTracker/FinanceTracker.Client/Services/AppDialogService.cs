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

        public Task<bool?> ConfirmDeleteCategory(string categoryName, int transactionCount)
        {
            var message = transactionCount > 0
                ? $"\"{categoryName}\" has {transactionCount} transaction{(transactionCount == 1 ? "" : "s")}. " +
                  $"They will be moved to \"Other\" before this category is deleted. This can't be undone."
                : $"This will permanently remove \"{categoryName}\". This can't be undone.";

            return _dialogService.Confirm(
                message,
                "Delete category?",
                new ConfirmOptions { OkButtonText = "Delete", CancelButtonText = "Cancel" }
            );
        }

        public Task<bool?> ConfirmDeactivate()
        {
            return _dialogService.Confirm(
                "You won't be able to access your account until you sign back in. Your data will be preserved.",
                "Deactivate account?",
                new ConfirmOptions { OkButtonText = "Deactivate", CancelButtonText = "Cancel" }
            );
        }
    }
}
