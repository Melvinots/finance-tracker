using FinanceTracker.Models;

namespace FinanceTracker.Repositories
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAllByUserAsync(int userId);
        Task<Transaction?> GetByIdAsync(int id, int userId);
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<Transaction> UpdateAsync(Transaction transaction);
        Task DeleteAsync(Transaction transaction);
    }
}
