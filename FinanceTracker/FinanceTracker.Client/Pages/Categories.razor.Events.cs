using FinanceTracker.Shared.DTOs.Categories;

namespace FinanceTracker.Client.Pages
{
    public partial class Categories
    {
        private void OpenCreateModal()
        {
            _selectedCategory = null;
            _showModal = true;
        }

        private void HandleEdit(CategoryDto category)
        {
            _selectedCategory = category;
            _showModal = true;
        }

        private void CloseModal()
        {
            _showModal = false;
            _selectedCategory = null;
        }

        private async Task HandleSave(SaveCategoryDto category)
        {   
            _isSaving = true;

            try
            {
                if (_selectedCategory is null)
                {
                    bool isDuplicate = _categories.Any(c => c.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase));

                    if (isDuplicate)
                    {
                        AppNotifier.Warning(summary: $"\"{category.Name}\" already exists.", duration: 50000);
                        return;
                    }

                    await CategoryService.CreateAsync(category);
                    AppNotifier.Success("Category created successfully.");
                }
                else
                {
                    await CategoryService.UpdateAsync(_selectedCategory.Id, category);
                    AppNotifier.Success("Category updated successfully.");
                }

                await LoadCategoriesAsync();
            }
            catch (Exception ex)
            {
                AppNotifier.Error(summary: "Failed to save category.", ex: ex);
            }
            finally
            {
                _isSaving = false;
                CloseModal();
            }
        }

        private async Task HandleDelete(CategoryDto category)
        {
            try
            {
                _categoryTransactionCount = await CategoryService.GetCountByCategoryAsync(category.Id);

                bool? confirmed = await AppDialogs.ConfirmDeleteCategory(category.Name, _categoryTransactionCount);

                if (confirmed == true)
                {
                    await CategoryService.DeleteAsync(category.Id);
                    _categories.Remove(category);

                    AppNotifier.Success("Category deleted successfully.");
                }
            }
            catch (Exception ex)
            {
                AppNotifier.Error(summary: "Failed to delete category.", ex: ex);
            }
        }
    }
}
