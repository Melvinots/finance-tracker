namespace FinanceTracker.Client.Components.Transactions
{
    public record TransactionFilter(string SearchTerm, TransactionTypeFilter Type, IReadOnlyCollection<int> CategoryIds);
}
