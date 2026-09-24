using FinanceTracker.Models;
using FinanceTracker.Repositories;
using FinanceTracker.Shared.DTOs.Transactions;

namespace FinanceTracker.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repo;

        public TransactionService(ITransactionRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<TransactionDto>> GetAllByUserAsync(int userId)
        {
            var transactions = await _repo.GetAllByUserAsync(userId);
            return transactions.Select(MapToDto).ToList();
        }

        public async Task<TransactionDto?> GetByIdAsync(int id, int userId)
        {
            var transaction = await _repo.GetByIdAsync(id, userId);
            return transaction is null ? null : MapToDto(transaction);
        }

        public async Task<TransactionDto> CreateAsync(SaveTransactionDto dto, int userId)
        {
            var category = await _repo.GetCategoryByIdAsync(dto.CategoryId, userId);

            if (category == null)
            {
                category = await _repo.GetOrCreateUncategorizedAsync(userId);
                dto.CategoryId = category.Id;
            }

            var transaction = new Transaction
            {
                Amount = dto.Amount ?? 0,
                Description = dto.Description,
                Date = dto.Date,
                IsExpense = dto.IsExpense,
                Notes = dto.Notes,
                CategoryId = dto.CategoryId,
                UserId = userId,
                Category = category
            };

            var result = await _repo.CreateAsync(transaction);
            return MapToDto(result);
        }

        public async Task<TransactionDto> UpdateAsync(int id, SaveTransactionDto dto, int userId)
        {
            var transaction = await _repo.GetByIdAsync(id, userId)
                ?? throw new KeyNotFoundException("Transaction not found.");

            transaction.Amount = dto.Amount ?? 0;
            transaction.Description = dto.Description;
            transaction.Date = dto.Date;
            transaction.IsExpense = dto.IsExpense;
            transaction.Notes = dto.Notes;

            if (transaction.CategoryId != dto.CategoryId)
            {
                var newCategory = await _repo.GetCategoryByIdAsync(dto.CategoryId, userId)
                    ?? throw new InvalidOperationException("Category not found.");

                transaction.CategoryId = dto.CategoryId;
                transaction.Category = newCategory;
            }

            var result = await _repo.UpdateAsync(transaction);
            return MapToDto(result);
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var transaction = await _repo.GetByIdAsync(id, userId)
                ?? throw new KeyNotFoundException("Transaction not found.");

            await _repo.DeleteAsync(transaction);
        }

        // ── private helpers ──────────────────────────────────────────

        private static TransactionDto MapToDto(Transaction t) => new()
        {
            Id = t.Id,
            Amount = t.Amount,
            Description = t.Description,
            Date = t.Date,
            IsExpense = t.IsExpense,
            Notes = t.Notes,
            CategoryId = t.CategoryId,
            CategoryName = t.Category.Name,
            CategoryColor = t.Category.Color,
            CategoryIcon = t.Category.Icon
        };
    }
}
