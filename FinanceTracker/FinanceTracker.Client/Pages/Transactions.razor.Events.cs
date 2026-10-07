using FinanceTracker.Client.Components.Transactions;
using static System.Net.WebRequestMethods;

namespace FinanceTracker.Client.Pages
{
    public partial class Transactions
    {
        private void HandleFilterChanged(TransactionFilter filter)
        {
            _filter = filter;
            ApplyFilters(filter);
        }

        private void ApplyFilters(TransactionFilter filter)
        {
            _filteredTransactions = _transactions
                .Where(t => MatchesSearch(t, filter.SearchTerm))
                .Where(t => MatchesType(t, filter.Type))
                .Where(t => MatchesCategories(t, filter.CategoryIds))
                .ToList();
        }

        private bool MatchesSearch(TransactionDto transaction, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return true;
            }

            return transaction.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
        }

        private bool MatchesType(TransactionDto transaction, TransactionTypeFilter selectedType)
        {
            return selectedType switch
            {
                TransactionTypeFilter.Income => !transaction.IsExpense,
                TransactionTypeFilter.Expense => transaction.IsExpense,
                _ => true
            };
        }

        private bool MatchesCategories(TransactionDto transaction, IReadOnlyCollection<int> categoryIds)
        {
            if (categoryIds == null || !categoryIds.Any())
            {
                return true;
            }

            return categoryIds.Contains(transaction.CategoryId);
        }

        private void OpenCreateModal()
        {
            _selectedTransaction = null;
            _showModal = true;
        }

        private void HandleEdit(TransactionDto transaction)
        {
            _selectedTransaction = transaction;
            _showModal = true;
        }

        private void CloseModal()
        {
            _showModal = false;
            _selectedTransaction = null;
        }

        private async Task HandleSave(SaveTransactionDto transaction)
        {
            try
            {
                if (_selectedTransaction is null)
                {
                    await TransactionService.CreateAsync(transaction);
                    AppNotifier.Success("Transaction added");
                }
                else
                {
                    await TransactionService.UpdateAsync(_selectedTransaction.Id, transaction);
                    AppNotifier.Success("Transaction updated");
                }

                await LoadTransactionsAsync();
            }
            catch (Exception ex)
            {
                AppNotifier.Error(summary: "Failed to save transaction", ex: ex);
            }
            finally
            {
                CloseModal();
            }
        }

        private async Task HandleDelete(TransactionDto transaction)
        {
            bool? confirmed = await AppDialogs.ConfirmDelete(transaction.Description, _itemType);

            if (confirmed == true)
            {
                try
                {
                    await TransactionService.DeleteAsync(transaction.Id);
                    _transactions.RemoveAll(t => t.Id == transaction.Id);

                    ApplyFilters(_filter);
                    AppNotifier.Success("Transaction deleted");
                }
                catch (Exception ex)
                {
                    AppNotifier.Error(summary: "Failed to delete transaction", ex: ex);
                }
            }
        }
    }
}
