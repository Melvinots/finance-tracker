using FinanceTracker.Shared.DTOs.Budgets;
using FinanceTracker.Shared.DTOs.Categories;
using Radzen;

namespace FinanceTracker.Client.Pages
{
    public partial class Budgets
    {
        [Inject] BudgetService BudgetService { get; set; } = default!;
        [Inject] CategoryService CategoryService { get; set; } = default!;
        [Inject] UserSettingsService UserSettingsService { get; set; } = default!;
        [Inject] AppNotificationService AppNotifier{ get; set; } = default!;
        [Inject] AppDialogService AppDialogs { get; set; } = default!;

        private bool _isLoading = true;

        private bool _showModal;

        private DateTime _selectedMonth = DateTime.Now;

        private string _currencyCode = string.Empty;

        private string _itemType = "budget";

        private List<BudgetDto> _budgets = new();

        private List<CategoryDto> _allCategories = new();

        private List<CategoryDto> _categories = new();

        private BudgetDto? _selectedBudget;
    }
}
