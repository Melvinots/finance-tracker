using FinanceTracker.Repositories;

namespace FinanceTracker.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repo;

        public AccountService(IAccountRepository repo)
        {
            _repo = repo;
        }

        public async Task<string> ExportDataAsync(int userId)
        {
            return await _repo.ExportDataAsync(userId);
        }

        public async Task DeactivateAccountAsync(int userId)
        {
            await _repo.DeactivateAccountAsync(userId);
        }
    }
}
