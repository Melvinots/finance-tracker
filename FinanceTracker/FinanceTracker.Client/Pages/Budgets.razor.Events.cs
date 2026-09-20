using FinanceTracker.Shared.DTOs.Budgets;

namespace FinanceTracker.Client.Pages
{
    public partial class Budgets
    {
        private void OpenCreateModal()
        {
            _selectedBudget = null;
            _showModal = true;
        }

        private void HandleEdit(BudgetDto budget)
        {
            _selectedBudget = budget;
            _showModal = true;
        }

        private void CloseModal()
        {
            _showModal = false;
            _selectedBudget = null;
        }

        private async Task HandleSave(SaveBudgetDto budget)
        {
            try
            {
                if (_selectedBudget is null)
                {
                    await BudgetService.CreateAsync(budget);
                    AppNotifier.Success("Budget added");
                }
                else
                {
                    await BudgetService.UpdateAsync(_selectedBudget.Id, budget);
                    AppNotifier.Success("Budget updated");
                }

                await LoadBudgetsAsync();
            }
            catch (Exception ex)
            {
                AppNotifier.Error(summary: "Failed to save budget", ex: ex);
            }
            finally
            {
                CloseModal();
                UpdateAvailableCategories();
            }
        }

        private async Task HandleDelete(BudgetDto budget)
        {
            bool? confirmed = await AppDialogs.ConfirmDelete(budget.CategoryName, _itemType);

            if (confirmed == true)
            {
                try
                {
                    await BudgetService.DeleteAsync(budget.Id);
                    _budgets.Remove(budget);

                    UpdateAvailableCategories();
                    AppNotifier.Success("Budget deleted");
                }
                catch (Exception ex)
                {
                    AppNotifier.Error(summary: "Failed to delete budget", ex: ex);
                }
            }
        }
    }
}
