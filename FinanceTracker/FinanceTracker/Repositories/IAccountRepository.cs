namespace FinanceTracker.Repositories
{
    public interface IAccountRepository
    {
        Task<string> ExportDataAsync(int userId);
        Task DeactivateAccountAsync(int userId);
    }
}
