namespace FinanceTracker.Client.Pages
{
    public partial class Transactions
    {
        private void ApplyFilters()
        {
            _filteredTransactions = _transactions
                .Where(MatchesSearch)
                .Where(MatchesType)
                .ToList();
        }

        private bool MatchesSearch(TransactionDto transaction)
        {
            if (string.IsNullOrWhiteSpace(_searchTerm))
            {
                return true;
            }

            return transaction.Description.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    transaction.CategoryName.Contains(_searchTerm, StringComparison.OrdinalIgnoreCase);
        }

        private bool MatchesType(TransactionDto transaction)
        {
            return _selectedType switch
            {
                "Income" => !transaction.IsExpense,
                "Expense" => transaction.IsExpense,
                _ => true
            };
        }

        private void OnSearchChanged(string value)
        {
            _searchTerm = value;
            ApplyFilters();
        }

        private void OnTypeChanged(string value)
        {
            _selectedType = value;
            ApplyFilters();
        }

        private void OpenCreateModal()
        {
            _selectedTransaction = null;
            _showModal = true;
        }

        private void OpenEditModal(TransactionDto transaction)
        {
            _selectedTransaction = transaction;
            _showModal = true;
        }

        private void CloseModal()
        {
            _showModal = false;
            _selectedTransaction = null;
        }

        private async Task SaveTransaction(SaveTransactionDto transaction)
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

        private async Task DeleteTransaction(TransactionDto transaction)
        {
            bool? confirmed = await AppDialogs.ConfirmDelete(transaction.Description, _itemType);

            if (confirmed == true)
            {
                try
                {
                    await TransactionService.DeleteAsync(transaction.Id);
                    _transactions.Remove(transaction);

                    ApplyFilters();
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
