using FinanceTracker.Client.Components.Transactions;
using FinanceTracker.Shared.DTOs.Categories;

namespace FinanceTracker.Client.Pages
{
    public partial class Transactions
    {
        [Inject] private TransactionService TransactionService { get; set; } = default!;
        [Inject] private CategoryService CategoryService { get; set; } = default!;
        [Inject] private UserSettingsService UserSettingsService { get; set; } = default!;
        [Inject] private AppNotificationService AppNotifier { get; set; } = default!;
        [Inject] private AppDialogService AppDialogs { get; set; } = default!;

        private bool _isLoading = true;

        private bool _showModal = false;

        private TransactionFilter _filter = new("", TransactionTypeFilter.All, []);

        private string _itemType = "transaction";

        private string _currencyCode = string.Empty;

        private List<TransactionDto> _transactions = new();

        private List<TransactionDto> _filteredTransactions = new();

        private List<CategoryDto> _categories = new();

        private TransactionDto? _selectedTransaction;
    }
}