using FinanceTracker.Shared.DTOs.Categories;

namespace FinanceTracker.Client.Pages
{
    public partial class Categories
    {
        [Inject] CategoryService CategoryService { get; set; } = default!;
        [Inject] AppNotificationService AppNotifier { get; set; } = default!;
        [Inject] AppDialogService AppDialogs { get; set; } = default!;

        private bool _isLoading = true;

        private bool _showModal = false;

        private bool _isSaving = false;

        private int _categoryTransactionCount = 0;

        private List<CategoryDto> _categories = new();

        private CategoryDto? _selectedCategory;
    }
}