using FinanceTracker.Shared.DTOs.Transactions;

namespace FinanceTracker.Services
{
    public interface ITransactionService
    {
        Task<List<TransactionDto>> GetAllByUserAsync(int userId);
        Task<TransactionDto?> GetByIdAsync(int id, int userId);
        Task<TransactionDto> CreateAsync(CreateTransactionDto dto, int userId);
        Task<TransactionDto> UpdateAsync(int id, UpdateTransactionDto dto, int userId);
        Task DeleteAsync(int id, int userId);
    }
}
