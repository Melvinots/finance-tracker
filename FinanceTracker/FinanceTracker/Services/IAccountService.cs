namespace FinanceTracker.Services
{
    public interface IAccountService
    {
        Task<string> ExportDataAsync(int userId);
        Task DeactivateAccountAsync(int userId);
    }
}
